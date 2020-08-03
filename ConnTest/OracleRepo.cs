using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;


namespace ConnTest
{
    public class OracleRepo
    {
        string connStr;

        public OracleRepo(string connStr)
        {
            this.connStr = connStr;
        }

        public void GetSomeThingResult()
        {
            #region  DATA for test
            #endregion

            var conn = new OracleConnection(connStr);

            string procedureName = "OVA.LEA_PRA.GetSomeThing";
            OracleCommand cmd = new OracleCommand(procedureName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            OracleParameter inputParam = cmd.Parameters.Add("IN_PARAM", OracleDbType.Clob);
            inputParam.Direction = ParameterDirection.Input;
            inputParam.Value = "input data";

            OracleParameter outParamOne = cmd.Parameters.Add("OUT_PARAM_1", OracleDbType.Clob);
            outParamOne.Direction = ParameterDirection.Output;

            OracleParameter outParamTwo = cmd.Parameters.Add("OUT_PARAM_2", OracleDbType.Clob);
            outParamTwo.Direction = ParameterDirection.Output;

            OracleParameter outParamRefCursor = cmd.Parameters.Add("OUT_PARAM_REFC", OracleDbType.RefCursor);
            outParamRefCursor.Direction = ParameterDirection.Output;

            try
            {
                conn.Open();

                ConnectionInfo(conn);

                cmd.ExecuteNonQuery();

                Console.WriteLine($"outParamOne = {((OracleClob)cmd.Parameters["OUT_PARAM_1"].Value).Value}");
                Console.WriteLine($"outParamTwo = {((OracleClob)cmd.Parameters["OUT_PARAM_2"].Value).Value}");
                Console.WriteLine($"outParamRafCursor = {cmd.Parameters["OUT_PARAM_REFC"].Value.GetType()}");

                Type type = cmd.Parameters["OUT_PARAM_REFC"].Value.GetType();

                Console.WriteLine(new string('-', 70));
                foreach (var item in type.GetProperties())
                {
                    Console.WriteLine($"Свойство - {item.Name} - {item.PropertyType}");
                }
                Console.WriteLine(new string('-', 70));

                var reader = ((OracleRefCursor)cmd.Parameters["OUT_PARAM_REFC"].Value).GetDataReader();

                //Считывание данных из RefCursor
                GetDataRefCursor(reader);

                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();

                Console.WriteLine("Произошла ошибка: \n" + exc.Message);
            }

        }


        private async void GetDataRefCursor(OracleDataReader reader)
        {
            try
            {
                if (reader.HasRows)
                {
                    int rowNumber = 1;
                    string rowStr = "";
                    while (await reader.ReadAsync())
                    {
                        rowStr += ($"------------ Row # {rowNumber++}  {new string('-', 90)} \n");

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            rowStr += ($"Field{i} Name: {reader.GetName(i)} - " +
                                       $"Value {reader[i]} - " +
                                       $"DataType : {reader[i].GetType().Name} \n");
                        }

                        rowStr += (new string('-', 115) + "\n\n");
                    }

                    Console.WriteLine(rowStr);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Произошла ошибка при вызове процедуры: \n" + exc.Message);
            }
        }

        private async void GetDataRefCursor1(OracleCommand cmd)
        {
            try
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        int rowNumber = 1;
                        while (await reader.ReadAsync())
                        {

                            Console.WriteLine($"------------ Строка # {rowNumber++} " + new string('-', 90));

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.WriteLine($"Field{i} Name: {reader.GetName(i),20} \t " +
                                                  $"Value: {reader[i],20} \t " +
                                                  $"DataType: {reader[i].GetType().Name,20} \t ");
                            }

                            Console.WriteLine(new string('-', 115));
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine("Произошла ошибка при вызове процедуры: \n" + exc.Message);
            }
        }

        private void ConnectionInfo(OracleConnection conn)
        {

            Console.WriteLine("Соедиенение: " + conn.State);
            Console.WriteLine("Время соединения: " + conn.ConnectionTimeout + "сек");
            Console.WriteLine("Доменное имя БД: " + conn.DatabaseDomainName);
            Console.WriteLine("Название издания базы данных: " + conn.DatabaseEditionName);
            Console.WriteLine("Название БД: " + conn.DatabaseName);
            Console.WriteLine("Строка соединения: " + conn.DataSource);
            Console.WriteLine("База данных: " + conn.Database);
            Console.WriteLine();
        }

        public void CheckConnection()
        {
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();
                    ConnectionInfo(conn);

                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                }

            }
        }
    }
}
