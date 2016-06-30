using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPN
{
    /// <summary>
    /// This class decalres/implements basic functionality common for all the nodes in CPN
    /// i.e. Arc management, token movement notifications
    /// </summary>
    public class Node 
    {
        private static Random random_gen_ = new Random();
		private static int id_counter = 0;

		private static int next_id() {
			//return random_gen_.Next();
			return id_counter++;
		}
        /// <summary>
        /// Maps ids of node into something human readable
        /// </summary>
        private static Dictionary<int, String> name_storage = new Dictionary<int, string>();

        private int _id;
        public int id { get { return _id; } }
        public String name_ { get { return name_storage[_id]; } set {name_storage[_id] = value; } }

		protected Dictionary<Node, Arc> input_arcs = new Dictionary<Node, Arc>();
		protected SortedList<Arc, Node> output_arcs = new SortedList<Arc, Node>();

        public Node()
        {
			_id = next_id();
        }

        public Node(String name)
        {
			_id = next_id();
            name_storage.Add(_id, name);
        }

        public virtual IEnumerable<Tuple> getTuples()
        {
            throw new NotImplementedException();
        }

        public virtual void  removeToken(Token token)
        {
            throw new NotImplementedException();
        }

        public void addInputArc(Arc arc)
        {
            input_arcs.Add(arc.from, arc);
        }

        public void addOutputArc(Arc arc)
        {
            output_arcs.Add(arc, arc.to);
        }

        protected void notifyOutputArcs()
        {
			//for(int i=shadow_out_arcs_index_array.Length-1;i>=0;i--){
			//    shadow_out_arcs_index_array[i].nodeAtInputHasChangedEvent();
			//}
			for (int i = output_arcs.Keys.Count - 1; i >= 0; i--) {
				output_arcs.Keys[i].nodeAtInputHasChangedEvent();
			}
                //foreach (Arc arc in output_arcs.Keys) {
                //    arc.nodeAtInputHasChangedEvent();
                //}
        }

        public virtual void arcHasChangedEvent(Arc arc)
        {
            throw new NotImplementedException();
        }

        //public static Arc operator >(Node a, Node b){
        //    if ((a is Transition)&&(b is Place)){
        //        return new Arc((Transition)a, (Place)b);
        //    }
        //    if ((a is Place)&&(b is Transition)){
        //        return new Arc((Place)a, (Transition)b);
        //    }
        //    throw new CPN.WrongConfigurationException("Invalid arc connection. Connect places to transitions (or reverse) only. You tried to connect: "+a.GetType ().Name + " to " + b.GetType ().Name);
        //}

        //public static Arc operator <(Node a, Node b)
        //{
        //    throw new CPN.WrongConfigurationException(" \"<\" operator is not supported. Use \">\" instead.");
        //}

    }
}
