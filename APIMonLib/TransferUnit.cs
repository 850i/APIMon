using System;
using System.Collections.Generic;

namespace APIMonLib {
    [Serializable]
    public class TransferUnit {
        //TODO IMPORTANT the next step in speed increase is indexing in TransferUnits. Looks like the most time takes computing of hash values over index strings
        private SortedDictionary<String, object> field_storage = null;

        public int Hook_sequence_number {
            get { return (int)this["SN"]; }
            set { this["SN"] = value; }
        }

        public TransferUnit() {
            field_storage = new SortedDictionary<string, object>();
            apiCallName = new APIFullName("undefined", "undefined");
            PID = -1;
            TID = -1;
        }

        public TransferUnit(TransferUnit tu) {
            field_storage = new SortedDictionary<string, object>(tu.field_storage);
        }


        public APIFullName apiCallName {
            get { return (APIFullName)field_storage["apiCallName"]; }
            set { field_storage["apiCallName"] = value; }
        }

        public int PID {
            get { return (int)field_storage["PID"]; }
            set { field_storage["PID"] = value; }
        }

        public int TID {
            get { return (int)field_storage["TID"]; }
            set { field_storage["TID"] = value; }
        }

        public object this[String index]   // Indexer declaration
        {
            get {
                try {
                    return field_storage[index];
                }catch(KeyNotFoundException){
                    string[] fields=getFields();
                    string fields_str=string.Empty;
                    for(int i=0;i<fields.Length;i++){
                        fields_str+=fields[i]+" ";
                    }
                    throw new KeyNotFoundException("Can not find key \"" + index + "\" among (" + fields_str + ")");
                }
            }
            set {
                if (index != null) {
                    if (value != null) {
                        field_storage[index] = value;
                    } else {
                        field_storage.Remove(index);
                    }
                }
            }
        }

        public bool fieldExists(string field) {
            return field_storage.ContainsKey(field);
        }

        /// <summary>
        /// This method returns all fields defined in current TransferUnit
        /// </summary>
        /// <returns></returns>
        public String[] getFields() {
            String[] result = new String[field_storage.Keys.Count];
            field_storage.Keys.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// This method loads into current transfer unit vaule of fields from transfer unit specified in parameters.
        /// In case there is a field with the same name in current transfer unit its value will be overwritten.
        /// </summary>
        /// <param name="field_names">list of filed names to load</param>
        /// <param name="prefix">prefix to append to filed name in new transfer unit. For example: prefix="file." then field "name" will be stored as "file.name"</param>
        /// <param name="tu"> transfer unit from where to load fields</param>
        public void loadFields(IEnumerable<string> field_names, string prefix, TransferUnit tu) {
            foreach (String key in field_names) {
                try {
                    field_storage.Add(prefix + key, tu[key]);
                } catch (ArgumentException) {
                    Console.WriteLine("The field [" + prefix + key + "] has been overwritten");
                    field_storage[prefix + key] = tu[key];
                }
            }
        }

        /// <summary>
        /// This method loads into current transfer unit value of fields from the transfer unit specified in parameters.
        /// In case there is a field with the same name in current transfer unit its value will be overwritten.
        /// </summary>
        /// <param name="prefix">prefix to append to filed name in new transfer unit. For example: prefix="file" then field "name" will be stored as "file.name"</param>
        /// <param name="tu"> transfer unit from where to load fields</param>
        public void loadAllFields(string prefix, TransferUnit tu) {
            loadFields(tu.field_storage.Keys, prefix, tu);
            //foreach (String key in tu.field_storage.Keys) {
            //    try {
            //        field_storage.Add(key, tu[key]);
            //    } catch (ArgumentException) {
            //        Console.WriteLine("The field [" + key + "] has been overwritten");
            //        field_storage[key] = tu[key];
            //    }
            //}
        }

        public override string ToString() {
            String result = "TransferUnit:";
            foreach (String key in field_storage.Keys) {
                result += "\n" + key + " = " + field_storage[key];
            }
            return result;
        }
    }
}
