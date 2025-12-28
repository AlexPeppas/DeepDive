using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DeepDiveTechnicals.Services
{
    public class TableManipulation
    {
        //public List<RequestDataItems> QueueRequestData {get;set;}
        //i: queueRequestData
        //o: QueueRequestData_Json
        public DataTable CreateTheTable()
        {
            System.Data.DataTable table = new DataTable("ParentTable");
            // Declare variables for DataColumn and DataRow objects.
            DataColumn column;
            DataRow row;

            /*// Create new DataColumn, set DataType,
            // ColumnName and add to DataTable.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "id";
            column.ReadOnly = true;
            column.Unique = true;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);*/

            // Create second column.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "value";
            column.AutoIncrement = false;
            column.Caption = "ParentItem";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);

            for (var i=0;i<2;i++)
            {
                row = table.NewRow();
                row["value"] = "text";
                table.Rows.Add(row);
            }
            return table;
        }
        public void Manipulate()
        {
            var types = CreateTheTable();
            string busJson = string.Empty;
            string processJson = string.Empty;
            string typesJson = string.Empty;

            PayloadObject payloadInstance = new PayloadObject();
            payloadInstance.Types = new List<string>();
            //var typesList = new List<Types>();

            for (int i = 0; i < types.Rows.Count; i++)
            {
                //Types typesObject = new Types();
                //typesObject.value = types.Rows[i]["value"].ToString();
                //typesList.Add(typesObject);
                payloadInstance.Types.Add(types.Rows[i]["value"].ToString());
                
            }
            payloadInstance.Types.Add("text");
            payloadInstance.Types.Add("number");
            var typesList = new { typesList = payloadInstance.Types };
            //typesJson = JsonConvert.SerializeObject(typesList);
            typesJson = JsonConvert.SerializeObject(new { payloadInstance.Types });
            typesJson = typesJson.Substring(typesJson.IndexOf('['), typesJson.Length - 1);
            //typesJson= payloadInstance.Types.ToString();//JsonConvert.SerializeObject(payloadInstance.Types);


            /*
                        //var processesList = new List<Processes>();
                        for (int i = 0; i < processes.Rows.Count; i++)
                        {
                            //Processes processObject = new Processes();
                            //processObject.value = processes.Rows[i]["value"].ToString();
                            //processesList.Add(processObject);
                            payloadInstance.Processes.Add(processes.Rows[i]["value"].ToString());
                        }
                        var processesList = new { processesList = payloadInstance.Processes };
                        processJson = JsonConvert.SerializeObject(processesList);
                        //processJson= payloadInstance.Processes.ToString();



                        //var busList = new List<BusinessUnits>();
                        for (int i = 0; i < businessUnits.Rows.Count; i++)
                        {
                            *//*BusinessUnits busObject = new BusinessUnits();
                            busObject.value = businessUnits.Rows[i]["value"].ToString();
                            busList.Add(busObject);
                            *//*
                            payloadInstance.BusinessUnits.Add(processes.Rows[i]["value"].ToString());
                        }
                        //busJson= payloadInstance.BusinessUnits.ToString();
                        var busList = new { busList = payloadInstance.BusinessUnits };
                        busJson = JsonConvert.SerializeObject(busList);*/
        }
    }
}
