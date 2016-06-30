using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIMonLib;
using APIMonLib.Hooks.ntdll.dll;
using APIMonLib.Hooks.kernel32.dll;
using APIMonLib.Hooks.ws2_32.dll;
using System.Diagnostics;
using CPN;

namespace APIMon {

	public enum Delete : byte {
		No = 0,
		Yes = 1
	}

	public enum Separate : byte {
		No = 0,
		Yes = 1
	}

	public class CPNetBlocks {
		#region Static fields and function fields

		private static CPNetBlocks cpnet_blocks = null;

		public static Func<object, string> to_string = (obj) => (string)obj;

		public static Func<int, string> get_process_module_name = (PID) => Process.GetProcessById(PID).MainModule.FileName;

		/// <summary>
		/// This functions checks if process with processID has been started from the module with name provided
		/// </summary>
		/// <param name="processID">ID of the process to check</param>
		/// <param name="module_name">module name to check</param>
		/// <returns></returns>
		public static bool checkProcessStartedFromTheModule(int processID, string module_name) {
			string process_module_name = get_process_module_name(processID);
			bool result = ends_with_string(module_name, process_module_name);
			return result;
		}

		public static Func<object, int> to_int = (obj) =>
		{
			try {
				return (int)obj;
			} catch (InvalidCastException) {
				throw new InvalidCastException("to_int conversion error of " + obj.ToString());
			}
		};

		public static Func<object, string, bool> begins_with_string = (obj, strng) => to_string(obj).StartsWith(strng, true, System.Globalization.CultureInfo.CurrentCulture);
		public static Func<object, string, bool> ends_with_string = (obj, strng) => to_string(obj).EndsWith(strng, true, System.Globalization.CultureInfo.CurrentCulture);
		public static Func<object, object, bool> strings_equal_ignore_case = (obj1, obj2) => to_string(obj1).ToUpper().Equals(to_string(obj2).ToUpper());
		public static Func<object, string> get_file_extention = (obj) => getFileExtention(to_string(obj));

		public static string getFileExtention(string file_full_path) {
			//todo replace this code with correct extention detection code. Treat files w/o extention and extentions not equal to 3 letters
			return file_full_path.Substring(file_full_path.Length - 3);
		}

		private static Func<Tuple, Token> STANDARD_GENEXP(Place base_place) {
			return tuple =>
			{
				Token result = new Token(tuple[base_place]);
				result.prevTuple = tuple;
				return result;
			};
		}

		private static Func<Token, bool> TRUE = token => true;
		#endregion

		#region Fields
		private Place zw_close_api = null;
		private Place closed_objects_place = null;
		private Place unrelated_close_calls = null;

		/// <summary>
		/// Ths dictionary contains places destroying tokens mapped by their names
		/// </summary>
		private Dictionary<string, Place> destroy_places = new Dictionary<string, Place>();

		private Random random = new Random();
		#endregion

		private CPNetBlocks() {
			defineZwCloseProcessing();
		}

		private static CPNetBlocks getInstance() {
			if (cpnet_blocks == null) cpnet_blocks = new CPNetBlocks();
			return cpnet_blocks;
		}

		/// <summary>
		/// This method intializes places and does intial configuration of object-closing sub CPN
		/// Then methods like assembleCloseHandleSubNet may work by attaching some place to object-closing sub CPN
		/// </summary>
		private void defineZwCloseProcessing() {
			zw_close_api = new ApiPlace(typeof(Hook_ZwClose));
			//zw_close_api.addPutReaction(new Place.Reaction(getPrintReactionProvider(ConsoleColor.DarkGray).simplePrintToken));
			closed_objects_place = new Place("Closed_ojects");
			unrelated_close_calls = new Place("All_close_calls");
			//unrelated_close_calls = getPlaceDestroingTokens("all_close_calls");

			CPNetBlocks.assembleMonoInputStructure(zw_close_api, Arc.MIN_WEIGHT, Delete.Yes, unrelated_close_calls, token => true).generating_expression = tuple => new Token(tuple[zw_close_api]);

			unrelated_close_calls.addPutReaction(new ActionReactionProvider().printCountPeriodically);
			unrelated_close_calls.addPutReaction(new ActionReactionProvider().freePlacePeriodically);

			closed_objects_place.addPutReaction(new ActionReactionProvider().printCountPeriodically);
			closed_objects_place.addPutReaction(new ActionReactionProvider().freePlacePeriodically);

			//Transition t_unrelated_close = new Transition("UnrelatedCloseCallsTransition");
			//t_unrelated_close.addUnaryExpression(zw_close_api, (token) =>true);

			//new Arc(zw_close_api, t_unrelated_close, Arc.MIN_WEIGHT).enableTokenRemovalFromInput();
			//new Arc(t_unrelated_close, unrelated_close_calls).enableTokenRemovalFromInput().generating_expression = tuple => new Token(tuple[zw_close_api]);
		}

		/// <summary>
		/// This function assembles subnet for the automatic removal of tokens from place based on their handle field.
		/// This subnet uses ZwClose place for closing handles. It removes token from input
		/// </summary>
		/// <param name="what_to_close">from which place to remove tokens upon ZwClose arrival</param>
		/// <param name="handle_name">the name of the handle field in the place</param>
		private void _assembleCloseHandleSubNet(Place what_to_close, String handle_name) {
			Transition t = new Transition("CloseTransition");
			t.addBinaryExpression(new Place[] { what_to_close, zw_close_api },
				(token1, token2) =>
				{
					return token1[handle_name].Equals(token2["handle"]);
				}
								  );
			new Arc(what_to_close, t).enableTokenRemovalFromInput();
			new Arc(zw_close_api, t);//.enableTokenRemovalFromInput();
			new Arc(t, closed_objects_place).generating_expression = tuple =>
			{
				Token result = new Token(tuple[what_to_close]);
				result.prevTuple = tuple;
				return result;
			};
		}

		/// <summary>
		/// This function assembles subnet for the automatic removal of tokens from place based on their handle field.
		/// This subnet uses ZwClose place for closing handles. It removes token from input
		/// </summary>
		/// <param name="what_to_close">from which place to remove tokens upon ZwClose arrival</param>
		/// <param name="handle_name">the name of the handle field in the place</param>
		public static void assembleCloseHandleSubNet(Place what_to_close, String handle_name) {
			getInstance()._assembleCloseHandleSubNet(what_to_close, handle_name);
		}

		/// <summary>
		/// Returns a new Place connected to handle-closing subCPN
		/// </summary>
		/// <param name="place_name">What name place should have</param>
		/// <param name="close_by_index">By what parameter place tokens should be closed e.g. FileHandle, SectionHandle, etc.</param>
		/// <returns></returns>
		public static Place getPlaceClosedByZwClose(string place_name, string close_by_index) {
			Place result = new Place(place_name);
			CPNetBlocks.getInstance()._assembleCloseHandleSubNet(result, close_by_index);
			return result;
		}


		public static ApiPlace getApiPlaceClosedByZwClose(Type type, string close_by_index) {
			ApiPlace result = new ApiPlace(type);
			CPNetBlocks.getInstance()._assembleCloseHandleSubNet(result, close_by_index);
			return result;
		}

		/// <summary>
		/// Assembles SISO structure with arbitrary guard expression. It returns Arc that connects Transition with output place s.t. it is possible to change generating expression there.
		/// </summary>
		/// <param name="_in">input place (token removed if remove_from_input==Yes)</param>
		/// <param name="remove_from_input">if set to Yes, the tokens from input place will be removed</param>
		/// <param name="_out">output place</param>
		/// <param name="transition_expression"></param>
		/// <returns></returns>
		public static Arc assembleMonoInputStructure(Place _in, Delete remove_from_input, Place _out, Func<Token, bool> transition_expression) {
			return assembleMonoInputStructure(_in, Arc.DEFAULT_WEIGHT, remove_from_input, _out, transition_expression);
		}

		/// <summary>
		/// Assembles SISO structure with arbitrary guard expression. It returns Arc that connects Transition with output place s.t. it is possible to change generating expression there.
		/// </summary>
		/// <param name="_in">input place (token removed if remove_from_input==Yes)</param>
		/// <param name="weight">the weight for the input arc from _in place</param>
		/// <param name="remove_from_input">if set to Yes, the tokens from input place will be removed</param>
		/// <param name="_out">output place </param>
		/// <param name="transition_expression"></param>
		/// <returns>output arc going to out place s.t. it is possible to redefine generating expression.</returns>
		public static Arc assembleMonoInputStructure(Place _in, int weight, Delete remove_from_input, Place _out, Func<Token, bool> transition_expression) {
			Transition t_mono_input = new Transition("MonoInputTransition(" + _in.name_ + ")=>" + _out.name_);
			t_mono_input.addUnaryExpression(_in, transition_expression);
			new Arc(_in, t_mono_input, weight).setTokenRemoval(remove_from_input.Equals(Delete.Yes));
			Arc output_arc = new Arc(t_mono_input, _out);
			//set default generating exression here
			output_arc.generating_expression = tuple => new Token(tuple[_in]);
			return output_arc;
		}

		/// <summary>
		/// This function assembles very common structure with two places at input of one transition and one output place.
		/// It returns Arc that connects Transition with output place s.t. it is possible to change generating expression there.
		/// </summary>
		/// <param name="_a">First input place</param>
		/// <param name="del_a">If set to true, tokens will be removed from the _a</param>
		/// <param name="_b">Second input place</param>
		/// <param name="del_b">If set to true, tokens will be removed from the _b</param>
		/// <param name="_out">Output place</param>
		/// <param name="transition_expression"></param>
		/// <returns>output arc going to out place s.t. it is possible to redefine generating expression.</returns>
		public static Arc assembleDiInputStructure(Place _a, Delete del_a, Place _b, Delete del_b, Place _out, Func<Token, Token, bool> transition_expression) {
			return assembleDiInputStructure(_a, Arc.DEFAULT_WEIGHT, del_a, _b, Arc.DEFAULT_WEIGHT, del_b, _out, transition_expression);
		}

		/// <summary>
		/// This function assembles very common structure with two places at input of one transition and one output place.
		/// It returns Arc that connects Transition with output place s.t. it is possible to change generating expression there.
		/// </summary>
		/// <param name="_a">First input place</param>
		/// <param name="a_weight">the weight for the input arc from _a place</param>
		/// <param name="del_a">If set to true, tokens will be removed from the _a</param>
		/// <param name="_b">Second input place</param>
		/// <param name="b_weight">the weight for the input arc from _b place</param>
		/// <param name="del_b">If set to true, tokens will be removed from the _b</param>
		/// <param name="_out">Output place</param>
		/// <param name="transition_expression"></param>
		/// <returns>output arc going to out place s.t. it is possible to redefine generating expression.</returns>
		public static Arc assembleDiInputStructure(Place _a, int a_weight, Delete del_a, Place _b, int b_weight, Delete del_b, Place _out, Func<Token, Token, bool> transition_expression) {
			Transition t_di_input = new Transition("DiInputTransition(" + _a.name_ + ", " + _b.name_ + ")=>" + _out.name_);
			t_di_input.addBinaryExpression(_a + _b, transition_expression);
			new Arc(_a, t_di_input, a_weight).setTokenRemoval(del_a.Equals(Delete.Yes));
			new Arc(_b, t_di_input, b_weight).setTokenRemoval(del_b.Equals(Delete.Yes));
			return new Arc(t_di_input, _out);
		}

		/// <summary>
		/// Joins two places into one w/o any conditions. Tokens are removed from input places
		/// </summary>
		/// <param name="_a">first input place (token removed)</param>
		/// <param name="_b">second input place (token removed)</param>
		/// <param name="_out"></param>
		public static void unconditionallyJoinPlacesIntoOne(Place _a, Place _b, Place _out) {
			Transition t_join_send_1 = new Transition("JoinUnconditionally_1(" + _a.name_ + ", " + _b.name_ + ")=>" + _out.name_);
			Transition t_join_send_2 = new Transition("JoinUnconditionally_2(" + _a.name_ + ", " + _b.name_ + ")=>" + _out.name_);
			new Arc(_a, t_join_send_1).enableTokenRemovalFromInput();
			new Arc(_b, t_join_send_2).enableTokenRemovalFromInput();
			new Arc(t_join_send_1, _out).generating_expression = tuple => new Token(tuple[_a]);
			new Arc(t_join_send_2, _out).generating_expression = tuple => new Token(tuple[_b]);
		}

		/// <summary>
		/// This class defines actions that have access to some additional place (side effect place) to
		/// perform some operation upon it.
		/// </summary>
		private class SideReactionProvider {
			public static string ID_FIELD_NAME = "id_code_for_side_reaction";
			private static Random random = new Random();
			private int counter = 0;

			private string id_field_name = string.Empty;

			//private Random rnd = new Random();

			private Place side_effect_place;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="side_effect_place">place onto side effects will be applied</param>
			public SideReactionProvider(Place side_effect_place) {
				this.side_effect_place = side_effect_place;
				id_field_name = ID_FIELD_NAME + "_" + random.Next(1000).ToString();
			}

			/// <summary>
			/// This action removes from side effect place a token by using ID comparison.
			/// Therefore all tokens in both places should have IDs. Use addIdFieldToToken action for that.
			/// </summary>
			/// <param name="place"></param>
			/// <param name="token"></param>
			public void removeFromSideEffectPlace(Place place, Token token) {
				Func<Token, Token, bool> equality_criterion = getEqualityCriterion();
				foreach (Token t in side_effect_place.getTokens()) {
					if (equality_criterion(token, t)) {
						side_effect_place.removeToken(t);
						break;
					}
				}
			}

			/// <summary>
			/// This action adds token to side effect place.
			/// </summary>
			/// <param name="place"></param>
			/// <param name="token"></param>
			public void addToSideEffectPlace(Place place, Token token) {
				side_effect_place.putToken(new Token(token));
			}

			/// <summary>
			/// This action generates new Id for token and adds this ID to the token structure
			/// s.t. later this token may be identified for removal in action removeFromOtherPlace
			/// </summary>
			/// <param name="place"></param>
			/// <param name="token"></param>
			public void addIdFieldToToken(Place place, Token token) {
				if (token.fieldExists(id_field_name)) 
					throw new ArgumentException("Can not add duplication detection field. The field exists already: " + id_field_name + " in token " + token.ToString());
				token[id_field_name] = counter++;
			}

			public Func<Token, Token, bool> getEqualityCriterion() {
				return (token1, token2) => token1[id_field_name].Equals(token2[id_field_name]);
			}
		}

		/// <summary>
		/// This method bonds _in and _out place in such way that any token that has been put into _in place
		/// is copied to _out place. Any token removed from _in place will be removed from _out place in case there is one.
		/// This method uses ILLEGAL according to the religion of CPN side effects for removeToken reaction for _in place. Be careful with self-dloop structures.
		/// Also be careful with remaining tokens. BE CAREFUL: It copies all contents of the _in place into _out place upon each
		/// arrival of new token into _in place.
		/// </summary>
		/// <param name="_in">mirror source</param>
		/// <param name="_out">mirror destination</param>
		public static void duplicatePlaceWithRemoval(Place _in, Place _out) {
			//define token removing side effects
			SideReactionProvider srp = new SideReactionProvider(_out);
			//Mark all tokens in 
			_in.addPutReaction(new Place.Reaction(srp.addIdFieldToToken));
			//Remove tokens with the same mark
			_in.addRemoveReaction(new Place.Reaction(srp.removeFromSideEffectPlace));
			duplicateOnlyNewTokens(_in, _out);
		}

		/// <summary>
		/// Assebles subCPN which mirrors _in place in _out place.
		/// That means when token comes into _in place it is copied into _out place. When token is removed from _in place
		/// it will NOT be removed from _out place
		/// </summary>
		/// <param name="_in"></param>
		/// <param name="_out"></param>
		public static void duplicateOnlyNewTokens(Place _in, Place _out) {
			//Define where side reaction will be applied
			SideReactionProvider srp = new SideReactionProvider(_out);
			_in.addPutReaction(new Place.Reaction(srp.addToSideEffectPlace));
		}

		/// <summary>
		/// Assembled subCPN which unconditionally deletes all tokens from the place
		/// This subCPN has lowest priority possbile. So some other arc with higher priority
		/// can intercept tokens before they will be deleted
		/// </summary>
		/// <param name="place_to_dump"></param>
		public static void dump(Place place_to_dump) {
			CPNetBlocks.assembleMonoInputStructure(place_to_dump,
															 Arc.MIN_WEIGHT,
															 Delete.Yes,
															 CPNetBlocks.getPlaceDestroingTokens(),
															 TRUE).generating_expression = STANDARD_GENEXP(place_to_dump);
		}

		/// <summary>
		/// Returns a place which mirrors source place. That means whenever token comes to source place
		/// the copy of this token will come to the resulting place. When
		/// token is removed from _in place it will NOT be removed from _out place.
		/// </summary>
		/// <param name="source">place to mirror</param>
		/// <returns>A new place which mirrors source place</returns>
		public static Place mirror(Place source) {
			Place result = new Place("Mirror_of_" + source.name_).setPrintLevel(Place.PrintLevel.Low);
			duplicateOnlyNewTokens(source, result);
			return result;
		}


		/// <summary>
		/// Assembles mirror of input place which has dumping arc with low priority
		/// So some other arc with higher priority can intercept tokens before they will be deleted
		/// </summary>
		/// <param name="place_to_mirror"></param>
		/// <returns></returns>
		public static Place mirrorDumped(Place place_to_mirror) {
			Place result = mirror(place_to_mirror);
			dump(result);
			return result;
		}


		/// <summary>
		/// Assembles mirror of input place s.t. when token is removed from orginal it is also removed from mirrored
		/// </summary>
		/// <param name="place_to_mirror"></param>
		/// <returns></returns>
		public static Place mirrorWithRemoval(Place source) {
			Place result = new Place("Mirror_R_of_" + source.name_).setPrintLevel(Place.PrintLevel.Low);
			duplicatePlaceWithRemoval(source, result);
			return result;
		}

		/// <summary>
		/// Passes to _out place only items that are not equal to any token in the _out place already.
		/// It REMOVES ALL tokens from _in place. 
		/// </summary>
		/// <param name="_in">input place</param>
		/// <param name="_out">output place where only new tokens should come</param>
		/// <param name="condition">equality criterion</param>
		public static void assembleEliminateDuplicateItems(Place _in, Place _out, Func<Token, Token, bool> condition) {
			assembleMonoInputStructure(_in, Arc.MIN_WEIGHT, Delete.Yes, _out, token =>
			{
				return true;
			});
			//Place elimination_place = new Place("Duplicates_eliminate_place(" + _in.name_ + "," + _out.name_ + ")");
			Place elimination_place = getPlaceDestroingTokens("duplicates_eliminate_place(" + _in.name_ + "," + _out.name_ + ")").setPrintLevel(Place.PrintLevel.Low);
			assembleDiInputStructure(_in,
										Arc.MAX_WEIGHT,
										Delete.Yes,
										_out,
										Arc.DEFAULT_WEIGHT,
										Delete.No,
										elimination_place,
										condition
									).generating_expression = tuple =>
			{
				Token result = new Token();
				result.prevTuple = tuple;
				return result;
			};
		}

		private const string DEAFULT_DESTROY_PLACE_NAME = "destroy_tokens_place";

		/// <summary>
		/// Returns place which destroys all otkens that come into it.
		/// </summary>
		/// <returns></returns>
		public static Place getPlaceDestroingTokens() {
			return getPlaceDestroingTokens(Separate.No);
		}

		/// <summary>
		/// Returns place which destroys all otkens that come into it.
		/// </summary>
		/// <param name="separate">Set to Yes when new place is needed. Set to No if cached place is needed</param>
		/// <returns></returns>
		public static Place getPlaceDestroingTokens(Separate separate) {
			if (separate == Separate.No) {
				return getPlaceDestroingTokens(DEAFULT_DESTROY_PLACE_NAME);
			} else {
				return getPlaceDestroingTokens(DEAFULT_DESTROY_PLACE_NAME + "_" + getInstance().random.Next());
			}
		}


		public static Place getPlaceDestroingTokens(string name) {
			return getInstance()._getPlaceDestroyingTokens(name);
		}

		/// <summary>
		/// Returns place which destroys all tokens that come into it.
		/// This method creates this place or fetches the place with the same name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private Place _getPlaceDestroyingTokens(string name) {
			if (destroy_places.ContainsKey(name)) {
				return destroy_places[name];
			}
			Place result = new Place(name).setPrintLevel(Place.PrintLevel.Low);
			ActionReactionProvider arp = new ActionReactionProvider();
			result.addPutReaction(arp.printCountPeriodically);
			result.addPutReaction(arp.freePlacePeriodically);
			destroy_places.Add(result.name_, result);
			return destroy_places[name];
		}


		private class ActionReactionProvider {

			private const int PLACE_PRINTING_POWER = 11;
			private const int PLACE_CLEANING_POWER = 13;

			private static Func<int, int> power2 = to_power => to_power <= 0 ? 1 : power2(to_power - 1) * 2;

			private int place_printing_period;
			private int place_clearance_period;

			public ActionReactionProvider() : this(PLACE_CLEANING_POWER, PLACE_PRINTING_POWER) { }

			public ActionReactionProvider(int log2_place_clearance_period, int log2_place_printing_period) {
				place_clearance_period = power2(log2_place_clearance_period);
				place_printing_period = power2(log2_place_printing_period);
			}

			public void warnIfMoreThanOne(Place place, Token token) {
				if (place.Count > 1) {
					ConsoleColor old = Console.BackgroundColor;
					Console.BackgroundColor = ConsoleColor.Red;
					Console.WriteLine("\nWARNING: Place " + place.name_ + " contains " + place.Count + " tokens");
					Console.BackgroundColor = old;
				}
			}

			public void printCountPeriodically(Place place, Token token) {
				if (place.Count % (place_printing_period) == 0) {
					ConsoleColor old = Console.BackgroundColor;
					Console.BackgroundColor = ConsoleColor.DarkBlue;
					Console.WriteLine("Place " + place.name_ + " has " + place.Count + " tokens");
					Console.BackgroundColor = old;
				}
			}

			public void freePlacePeriodically(Place place, Token token) {
				if (place.Count % place_clearance_period == 0) {
					ConsoleColor old = Console.BackgroundColor;
					Console.BackgroundColor = ConsoleColor.DarkMagenta;
					Console.WriteLine("Cleaning place " + place.name_ + " with " + place.Count + " tokens");
					Console.BackgroundColor = old;
					place.flushAllTokens();
				}
			}


		}
	}
}

#region commented out methods

///// <summary>
///// Copies incoming tokens into output place. BE CAREFUL: It copies all contents of the _in place into _out place upon each
///// arrival of new token into _in place.
///// </summary>
///// <param name="_in">source</param>
///// <param name="_out">destination</param>
//private static void duplicatePlaceNoRemoval(Place _in, Place _out) {
//    Transition t_arrival_replicator = new Transition("t_duplicate_place_replicator (" + _in + ", " + _out + ")");
//    t_arrival_replicator.addUnaryExpression(_in, token => true);
//    new Arc(_in, t_arrival_replicator, Arc.MIN_WEIGHT).setTokenRemoval(false);
//    new Arc(t_arrival_replicator, _out).generating_expression = tuple => new Token(tuple[_in]);
//}

///// <summary>
///// Assebles subCPN which mirrors _in place in _out place.
///// That means when token comes into _in place it is copied into _out place. When token is removed from _in place
///// it will NOT be removed from _out place
///// </summary>
///// <param name="_in"></param>
///// <param name="_out"></param>
//public static void duplicateOnlyNewTokens(Place _in, Place _out) {

//    #region first we copy incoming tokens into temporary place from where we can delete tokens

//    Place tmp_place = new Place("replicator_in_place(" + _in.name_ + "," + _out.name_ + ")").setPrintLevel(Place.PrintLevel.Low);
//    duplicatePlaceNoRemoval(_in, tmp_place);

//    #endregion

//    #region Remove tokens from internal storage when it is removed from mirrored place.

//    Place storage_place = new Place("replicator_storage_place(" + _in.name_ + "," + _out.name_ + ")").setPrintLevel(Place.PrintLevel.Low);
//    //Define where side reaction will be applied
//    SideReactionProvider srp = new SideReactionProvider(storage_place);
//    //Mark all tokens in 
//    _in.addPutReaction(new Place.Reaction(srp.addIdFieldToToken));
//    //delete them by marks if it is deleted from input
//    _in.addRemoveReaction(new Place.Reaction(srp.removeFromSideEffectPlace));

//    #endregion

//    #region We block duplicate tokens by sending them to dump
//    // if there is something in storage already that equal to new token We send this incoming token into dump before it will go to duplicator


//    Place dump_place = getPlaceDestroingTokens("replicator_dump_place(" + _in.name_ + "," + _out.name_ + ")").setPrintLevel(Place.PrintLevel.Low);
//    //Place dump_place = new Place("replicator_dump_place(" + _in.name_ + "," + _out.name_ + ")");

//    Transition t_duplicates_eliminator = new Transition("t_duplicates_eliminator (" + _in.name_ + ", " + _out.name_ + ")");
//    t_duplicates_eliminator.addBinaryExpression(tmp_place + storage_place, srp.getEqualityCriterion());
//    new Arc(tmp_place, t_duplicates_eliminator, Arc.MAX_WEIGHT).enableTokenRemovalFromInput();
//    new Arc(storage_place, t_duplicates_eliminator);
//    new Arc(t_duplicates_eliminator, dump_place).generating_expression = tuple =>
//    {
//        Token result = new Token();
//        result.prevTuple = tuple;
//        return result;
//    };
//    #endregion

//    #region Duplicate tokens from tmp place into output and storage if there is nothing in storage like that
//    Transition t_in_duplicator = new Transition("t_in_duplicator(" + _in + "," + _out + ")");
//    t_in_duplicator.addUnaryExpression(tmp_place, token => true);
//    new Arc(tmp_place, t_in_duplicator, Arc.MIN_WEIGHT).enableTokenRemovalFromInput();

//    new Arc(t_in_duplicator, _out).generating_expression = tuple =>
//    {
//        Token result = new Token(tuple[tmp_place]);
//        return result;
//    };
//    new Arc(t_in_duplicator, storage_place).generating_expression = tuple =>
//    {
//        Token result = new Token(tuple[tmp_place]);
//        return result;
//    };

//    #endregion
//}
#endregion