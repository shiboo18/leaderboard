using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Data.OleDb;
using System.Configuration;

namespace GPlusQuickstartCsharp
{
    public class connection_obj
    {
        public String readGstepsByDay(DateTime date)
        {

            String json = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";
                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT ROW_NUMBER() OVER(ORDER BY max_steps DESC) AS position,username,max_steps,last_updated_date FROM [gfit_steps_tbl] WHERE [fitness_date] = ? ", conn);
                // Add the parameters.
                OleDbParameter dateTimeParam = new OleDbParameter("@0", System.Data.SqlDbType.DateTime);
                dateTimeParam.Value = date;
                command.Parameters.Add(dateTimeParam);

                var details = new List<Dictionary<string, object>>();
                int count = 0;

                using (OleDbDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var dict = new Dictionary<string, object>();
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                if (rdr.GetName(i) == "last_updated_date")
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetDateTime(i).ToString());
                                }
                                else
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetValue(i));
                                }
                            }
                            details.Add(dict);
                            count++;
                        }
                    }
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json1 = jss.Serialize(details);
                //json = "{data:" + json1 + ",itemsCount:" + count.ToString() + "}";
                json = json1;
                conn.Close();

            }
            return json;
        }

        public String readCumulativeGstepsByDay()
        {

            String json = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";
                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT username,sum(max_steps) as steps FROM [gfit_steps_tbl] group by username order by steps desc ", conn);
                
                var details = new List<Dictionary<string, object>>();
                int count = 1;

                using (OleDbDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var dict = new Dictionary<string, object>();
                            dict.Add("position", count);
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                if (rdr.GetName(i) == "last_updated_date")
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetDateTime(i).ToString());
                                }
                                else
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetValue(i));
                                }
                            }
                            details.Add(dict);
                            count++;
                        }
                    }
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json1 = jss.Serialize(details);                
                json = json1;
                conn.Close();

            }
            return json;
        }

        public String readGStepsByUser(String emailid)
        {

            String json = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";
                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT TOP 7 [max_steps],[fitness_date] FROM [gfit_steps_tbl] where email_id = ? order by [fitness_date] desc", conn);
                // Add the parameters.
                command.Parameters.AddWithValue("@0", emailid);

                var details = new List<Dictionary<string, object>>();
                int count = 0;

                using (OleDbDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var dict = new Dictionary<string, object>();
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                if (rdr.GetName(i) == "fitness_date")
                                {
                                    dict.Add("label", rdr.IsDBNull(i) ? null : rdr.GetDateTime(i).ToShortDateString());
                                }
                                else
                                {
                                    dict.Add("y", rdr.IsDBNull(i) ? null : rdr.GetValue(i));
                                }
                            }
                            details.Add(dict);
                            count++;
                        }
                    }
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json1 = jss.Serialize(details);
                //json = "{data:" + json1 + ",itemsCount:" + count.ToString() + "}";
                json = json1;
                conn.Close();

            }
            return json;
        }

        public String readusername(String email)
        {

            String username = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                //conn.ConnectionString = "Server=SHIBOO\\SQLEXPRESS;Database=fitnessDb;User Id=testuser;Password=test;";
                // conn.ConnectionString = ConfigurationManager.AppSettings["DBConnString"];
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";

                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT username FROM [user] WHERE emailid = ?", conn);
                // Add the parameters.
                command.Parameters.AddWithValue("@0", email);
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        username = String.Format("{0}", reader[0]);

                    }
                }
                conn.Close();

            }
            return username;
        }

        public String readusergoal(String email)
        {

            String usergoal = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                //conn.ConnectionString = "Server=SHIBOO\\SQLEXPRESS;Database=fitnessDb;User Id=testuser;Password=test;";
                //conn.ConnectionString = ConfigurationManager.AppSettings["DBConnString"];
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";

                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT user_goal FROM [user] WHERE emailid = ?", conn);
                // Add the parameters.
                command.Parameters.AddWithValue("@0", email);
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usergoal = String.Format("{0}", reader[0]);

                    }
                }
                conn.Close();

            }
            return usergoal;
        }

        public void inseruserloggingactivity(String email)
        {

            Guid guid = Guid.NewGuid();
            string id = guid.ToString();
            DateTime now = DateTime.Now;
            using (OleDbConnection conn = new OleDbConnection())
            {
                //conn.ConnectionString = "Server=SHIBOO\\SQLEXPRESS;Database=fitnessDb;User Id=testuser;Password=test;";
                //conn.ConnectionString = ConfigurationManager.AppSettings["DBConnString"];
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";

                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("INSERT INTO [user_logs]([id],[username],[logging_in],[updated_by])VALUES(?,?,?,?)", conn);
                // Add the parameters.
                command.Parameters.AddWithValue("@0", id);
                command.Parameters.AddWithValue("@1", email);
                OleDbParameter fitnessDateTimeParam = new OleDbParameter("@2", System.Data.SqlDbType.DateTime);
                fitnessDateTimeParam.Value = now;
                command.Parameters.Add(fitnessDateTimeParam);
                command.Parameters.AddWithValue("@3", "shef");

                command.ExecuteNonQuery();
                conn.Close();

            }
            return;
        }

        public void insertGStepsdata(IList<stepDataPoint> steplist, String username, String emailid)
        {

            Guid guid = Guid.NewGuid();
            string id = guid.ToString();
            DateTime now = DateTime.Now;



            using (OleDbConnection conn = new OleDbConnection())
            {
                //conn.ConnectionString = "Server=SHIBOO\\SQLEXPRESS;Database=fitnessDb;User Id=testuser;Password=test;";
                // conn.ConnectionString = ConfigurationManager.AppSettings["DBConnString"];
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";

                conn.Open();
                foreach (var i in steplist)
                {
                    Console.WriteLine(i.MaxStep);
                    OleDbCommand cmdCount = new OleDbCommand("SELECT count(*) from [gfit_steps_tbl] WHERE fitness_date = ? and [email_id] = ?", conn);
                    OleDbParameter dateTimeParam = new OleDbParameter("@0", System.Data.SqlDbType.DateTime);
                    dateTimeParam.Value = i.Stamp;
                    cmdCount.Parameters.Add(dateTimeParam);
                    cmdCount.Parameters.AddWithValue("@1", emailid);
                    int count = (int)cmdCount.ExecuteScalar();
                    OleDbCommand command;
                    if (count == 1)
                    {
                        command = new OleDbCommand("UPDATE [gfit_steps_tbl] SET max_steps = ?, [last_updated_date] = ?,[updated_by] =? WHERE[fitness_date] =? and [email_id] = ?", conn);


                        if (i.Step == null)
                            command.Parameters.AddWithValue("@4", i.MaxStep);
                        else
                            command.Parameters.AddWithValue("@4", i.Step);
                        OleDbParameter nowDateTimeParam = new OleDbParameter("@5", System.Data.SqlDbType.DateTime);
                        nowDateTimeParam.Value = now;
                        command.Parameters.Add(nowDateTimeParam);
                        command.Parameters.AddWithValue("@6", "shef");
                        OleDbParameter fitnessDateTimeParam = new OleDbParameter("@3", System.Data.SqlDbType.DateTime);
                        fitnessDateTimeParam.Value = i.Stamp;
                        command.Parameters.Add(fitnessDateTimeParam);
                        command.Parameters.AddWithValue("@2", emailid);
                    }
                    else
                    {
                        // Create the command
                        command = new OleDbCommand("INSERT INTO [gfit_steps_tbl] ([id],[username],[email_id],[fitness_date],[max_steps],[last_updated_date],[updated_by])VALUES(?,?,?,?,?,?,?)", conn);
                        command.Parameters.AddWithValue("@0", id);
                        command.Parameters.AddWithValue("@1", username);
                        command.Parameters.AddWithValue("@2", emailid);
                        OleDbParameter fitnessDateTimeParam = new OleDbParameter("@3", System.Data.SqlDbType.DateTime);
                        fitnessDateTimeParam.Value = i.Stamp;
                        command.Parameters.Add(fitnessDateTimeParam);
                        if (i.Step == null)
                            command.Parameters.AddWithValue("@4", i.MaxStep);
                        else
                            command.Parameters.AddWithValue("@4", i.Step);
                        OleDbParameter nowDateTimeParam = new OleDbParameter("@5", System.Data.SqlDbType.DateTime);
                        nowDateTimeParam.Value = now;
                        command.Parameters.Add(nowDateTimeParam);
                        command.Parameters.AddWithValue("@6", "shef");
                    }


                    command.ExecuteNonQuery();
                }
                conn.Close();

            }

        }

        internal void updateStepGoal(int goal, String username, String emailid)
        {
            DateTime now = DateTime.Now;
            using (OleDbConnection conn = new OleDbConnection())
            {
                //conn.ConnectionString = "Server=SHIBOO\\SQLEXPRESS;Database=fitnessDb;User Id=testuser;Password=test;";
                //conn.ConnectionString = ConfigurationManager.AppSettings["DBConnString"];
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";

                conn.Open();


                // Create the command
                OleDbCommand command = new OleDbCommand("UPDATE [user] SET [user_goal]= ?,last_updated = ?, modified_by =? where [emailid] = ?", conn);
                // Add the parameters.
                command.Parameters.AddWithValue("@0", goal);
                OleDbParameter nowDateTimeParam = new OleDbParameter("@1", System.Data.SqlDbType.DateTime);
                nowDateTimeParam.Value = now;
                command.Parameters.Add(nowDateTimeParam);
                command.Parameters.AddWithValue("@2", "shef");
                command.Parameters.AddWithValue("@3", emailid);
                command.ExecuteNonQuery();

                conn.Close();

            }

        }

        internal void insertUserStepsData(DateTime userDate, string steps, string nickname, string emailid)
        {
            Guid guid = Guid.NewGuid();
            string id = guid.ToString();
            DateTime now = DateTime.Now;
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";
                conn.Open();
                OleDbCommand cmdCount = new OleDbCommand("SELECT count(*) from [user_steps_tbl] WHERE fitness_date = ? and [email_id] = ?", conn);
                OleDbParameter dateTimeParam = new OleDbParameter("@0", System.Data.SqlDbType.DateTime);
                dateTimeParam.Value = userDate;
                cmdCount.Parameters.Add(dateTimeParam);
                cmdCount.Parameters.AddWithValue("@1", emailid);
                int count = (int)cmdCount.ExecuteScalar();
                OleDbCommand command;
                if (count == 1)
                {
                    command = new OleDbCommand("UPDATE [user_steps_tbl] SET max_steps = ?, [last_updated_date] = ?,[updated_by] =? WHERE[fitness_date] =? and [email_id] = ?", conn);
                    command.Parameters.AddWithValue("@4", steps);
                    OleDbParameter nowDateTimeParam = new OleDbParameter("@5", System.Data.SqlDbType.DateTime);
                    nowDateTimeParam.Value = now;
                    command.Parameters.Add(nowDateTimeParam);
                    command.Parameters.AddWithValue("@6", "shef");
                    OleDbParameter fitnessDateTimeParam = new OleDbParameter("@3", System.Data.SqlDbType.DateTime);
                    fitnessDateTimeParam.Value = userDate;
                    command.Parameters.Add(fitnessDateTimeParam);
                    command.Parameters.AddWithValue("@2", emailid);
                }
                else
                {
                    // Create the command
                    command = new OleDbCommand("INSERT INTO [user_steps_tbl] ([id],[username],[email_id],[fitness_date],[max_steps],[last_updated_date],[updated_by])VALUES(?,?,?,?,?,?,?)", conn);
                    command.Parameters.AddWithValue("@0", id);
                    command.Parameters.AddWithValue("@1", nickname);
                    command.Parameters.AddWithValue("@2", emailid);
                    OleDbParameter fitnessDateTimeParam = new OleDbParameter("@3", System.Data.SqlDbType.DateTime);
                    fitnessDateTimeParam.Value = userDate;
                    command.Parameters.Add(fitnessDateTimeParam);
                    command.Parameters.AddWithValue("@4", steps);
                    OleDbParameter nowDateTimeParam = new OleDbParameter("@5", System.Data.SqlDbType.DateTime);
                    nowDateTimeParam.Value = now;
                    command.Parameters.Add(nowDateTimeParam);
                    command.Parameters.AddWithValue("@6", "shef");
                }
                command.ExecuteNonQuery();
                conn.Close();

            }

        }

        internal string readManaulStepsByUser(string emailid)
        {
            String json = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";
                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT TOP 7 [max_steps],[fitness_date] FROM [user_steps_tbl] where email_id = ? order by [fitness_date] desc", conn);
                // Add the parameters.
                command.Parameters.AddWithValue("@0", emailid);

                var details = new List<Dictionary<string, object>>();
                int count = 0;

                using (OleDbDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var dict = new Dictionary<string, object>();
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                if (rdr.GetName(i) == "fitness_date")
                                {
                                    dict.Add("label", rdr.IsDBNull(i) ? null : rdr.GetDateTime(i).ToShortDateString());
                                }
                                else
                                {
                                    dict.Add("y", rdr.IsDBNull(i) ? null : rdr.GetValue(i));
                                }
                            }
                            details.Add(dict);
                            count++;
                        }
                    }
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json1 = jss.Serialize(details);
                //json = "{data:" + json1 + ",itemsCount:" + count.ToString() + "}";
                json = json1;
                conn.Close();

            }
            return json;
        }

        internal string readManualStepsByDay(DateTime date)
        {
            String json = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";
                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT ROW_NUMBER() OVER(ORDER BY max_steps DESC) AS position,username,max_steps,last_updated_date FROM [user_steps_tbl] WHERE [fitness_date] = ? ", conn);
                // Add the parameters.
                OleDbParameter dateTimeParam = new OleDbParameter("@0", System.Data.SqlDbType.DateTime);
                dateTimeParam.Value = date;
                command.Parameters.Add(dateTimeParam);

                var details = new List<Dictionary<string, object>>();
                int count = 0;

                using (OleDbDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var dict = new Dictionary<string, object>();
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                if (rdr.GetName(i) == "last_updated_date")
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetDateTime(i).ToString());
                                }
                                else
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetValue(i));
                                }
                            }
                            details.Add(dict);
                            count++;
                        }
                    }
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json1 = jss.Serialize(details);
                //json = "{data:" + json1 + ",itemsCount:" + count.ToString() + "}";
                json = json1;
                conn.Close();

            }
            return json;
        }

        internal string readTempManaulSteps(string emailid)
        {
            DateTime userDate = DateTime.Now;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime now = DateTime.UtcNow;
            DateTime xTimeConverted = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone);
            DateTime today = new DateTime(xTimeConverted.Year, xTimeConverted.Month, xTimeConverted.Day, 0, 0, 0);
            DateTime yesterday = today.AddDays(-1);
            String json = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";
                conn.Open();

                // Create the command
               // OleDbCommand command = new OleDbCommand("SELECT fitness_date,max_steps,last_updated_date FROM [user_steps_tbl] WHERE ([fitness_date] = ? OR [fitness_date] = ?)AND [email_id] = ?", conn);
                // Add the parameters.
                /*OleDbParameter dateTimeParam = new OleDbParameter("@0", System.Data.SqlDbType.DateTime);
                dateTimeParam.Value = today;
                command.Parameters.Add(dateTimeParam);
                OleDbParameter dateTimeParam2 = new OleDbParameter("@1", System.Data.SqlDbType.DateTime);
                dateTimeParam2.Value = yesterday;
                command.Parameters.Add(dateTimeParam2);*/
                OleDbCommand command = new OleDbCommand("SELECT fitness_date,max_steps,last_updated_date FROM [user_steps_tbl] WHERE [email_id] = ? order by fitness_date desc", conn);
                command.Parameters.AddWithValue("@2", emailid);
                var details = new List<Dictionary<string, object>>();
                int count = 0;

                using (OleDbDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var dict = new Dictionary<string, object>();
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                if (rdr.GetName(i) == "last_updated_date" )
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetDateTime(i).ToString());
                                }
                                else if (rdr.GetName(i) == "fitness_date")
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetDateTime(i).ToShortDateString());
                                }
                                else
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetValue(i));
                                }
                            }
                            details.Add(dict);
                            count++;
                        }
                    }
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json1 = jss.Serialize(details);
                //json = "{data:" + json1 + ",itemsCount:" + count.ToString() + "}";
                json = json1;
                conn.Close();

            }
            return json;
        }
        public String readCumulativeManualstepsByDay()
        {

            String json = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";
                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT username,sum(max_steps) as steps FROM [user_steps_tbl] group by username order by steps desc ", conn);

                var details = new List<Dictionary<string, object>>();
                int count = 1;

                using (OleDbDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            var dict = new Dictionary<string, object>();
                            dict.Add("position", count);
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                if (rdr.GetName(i) == "last_updated_date")
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetDateTime(i).ToString());
                                }
                                else
                                {
                                    dict.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetValue(i));
                                }
                            }
                            details.Add(dict);
                            count++;
                        }
                    }
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json1 = jss.Serialize(details);
                json = json1;
                conn.Close();

            }
            return json;
        }
        internal string getUserAccountStatus(string emailid)
        {
            String accountStatus = "";
            using (OleDbConnection conn = new OleDbConnection())
            {
                //conn.ConnectionString = "Server=SHIBOO\\SQLEXPRESS;Database=fitnessDb;User Id=testuser;Password=test;";
                // conn.ConnectionString = ConfigurationManager.AppSettings["DBConnString"];
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";

                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("SELECT account_status FROM [user] WHERE emailid = ?", conn);
                // Add the parameters.
                command.Parameters.AddWithValue("@0", emailid);
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accountStatus = String.Format("{0}", reader[0]);

                    }
                }
                conn.Close();

            }
            return accountStatus;
        }

        internal void addUser(string newNickname, string newEmailId, string newGoal, string newAccountStatus)
        {
            Guid guid = Guid.NewGuid();
            string id = guid.ToString();
            DateTime now = DateTime.Now;
            using (OleDbConnection conn = new OleDbConnection())
            {
                //conn.ConnectionString = "Server=SHIBOO\\SQLEXPRESS;Database=fitnessDb;User Id=testuser;Password=test;";
                //conn.ConnectionString = ConfigurationManager.AppSettings["DBConnString"];
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";

                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("INSERT INTO [user]([id],[username],[emailid],[last_updated],[modified_by],[account_status],[user_goal])VALUES(?,?,?,?,?,?,?)", conn);
                // Add the parameters.
                command.Parameters.AddWithValue("@0", id);
                command.Parameters.AddWithValue("@1", newNickname);
                command.Parameters.AddWithValue("@2", newEmailId);
                OleDbParameter dateTimeParam = new OleDbParameter("@3", System.Data.SqlDbType.DateTime);
                dateTimeParam.Value = now;
                command.Parameters.Add(dateTimeParam);
                command.Parameters.AddWithValue("@4", "shef");
                command.Parameters.AddWithValue("@5", newAccountStatus);
                command.Parameters.AddWithValue("@6", newGoal);
                command.ExecuteNonQuery();
                conn.Close();

            }
            return;
        }

        internal void updateUserStatus(string newNickname, string newAccountStatus)
        {
            DateTime now = DateTime.Now;
            using (OleDbConnection conn = new OleDbConnection())
            {
                //conn.ConnectionString = "Server=SHIBOO\\SQLEXPRESS;Database=fitnessDb;User Id=testuser;Password=test;";
                //conn.ConnectionString = ConfigurationManager.AppSettings["DBConnString"];
                conn.ConnectionString = "Provider=SQLOLEDB;Server=ecnmssqldev.ecn.purdue.edu;Database=fitleaderboards;Integrated Security=SSPI";

                conn.Open();

                // Create the command
                OleDbCommand command = new OleDbCommand("UPDATE [user] SET [last_updated] = ?,[modified_by] = ?,[account_status] = ? WHERE username = ?", conn);
                // Add the parameters.           
                
                OleDbParameter dateTimeParam = new OleDbParameter("@0", System.Data.SqlDbType.DateTime);
                dateTimeParam.Value = now;
                command.Parameters.Add(dateTimeParam);
                command.Parameters.AddWithValue("@1", "shef");
                command.Parameters.AddWithValue("@2", newAccountStatus);
                command.Parameters.AddWithValue("@3", newNickname);
                command.ExecuteNonQuery();
                conn.Close();

            }
            return;
        }
    }
}