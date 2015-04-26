﻿using System;
using MySql.Data.MySqlClient;
using VacationMasters.Essentials;
using System.Collections.Generic;

namespace VacationMasters.Wrappers
{
    public class DbWrapper : IDbWrapper
    {
        private MySqlConnection CreateConnection()
        {
            var connection = new MySqlConnection("server=galactica.emanuelscirlet.com;database=vacationmasters;uid=sa;password=vacationmasters12;");
            return connection;
        }

        private MySqlCommand CreateCommand(MySqlConnection connection)
        {
            var command = new MySqlCommand { Connection = connection };
            return command;
        }

        public MySqlConnection GetConnection()
        {
            var connection = CreateConnection();
            connection.Open();
            return connection;
        }

        public MySqlCommand GetCommand(MySqlConnection connection)
        {
            var command = CreateCommand(connection);
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            return command;
        }

        private async void MessageBox(string msg)
        {
            var msgDlg = new Windows.UI.Popups.MessageDialog(msg) { DefaultCommandIndex = 1 };
            await msgDlg.ShowAsync();
        }

        public void RunCommand(Action<MySqlCommand> func)
        {
            RunCommand(c =>
            {
                func(c);
                return true;
            });
        }

        public T RunCommand<T>(Func<MySqlCommand, T> func)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    using (var command = GetCommand(connection))
                    {
                        return func(command);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox("An error occured!!\n" + e.Message);
                return default(T);
            }
        }

        public T QueryValue<T>(string sqlQuery)
        {
            return RunCommand(command =>
            {
                command.CommandText = sqlQuery;
                var result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return default(T);
                }
                Type t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                return (T)Convert.ChangeType(result, t);
            });
        }
        public List<Preference> GetAllPreferences()
        {
            return RunCommand(command =>
            {
                command.CommandText = "Select * from Preferences";
                var reader = command.ExecuteReader();
                var list = new List<Preference>();
                while (reader.Read())
                {
                    var preference = new Preference();
                    preference.ID = reader.GetInt32(0);
                    preference.Name = reader.GetString(1);
                    preference.Category = reader.GetString(2);
                    list.Add(preference);
                }
                return list;
            });
        }
        public List<String> GetPreferences()
        {
            return RunCommand(command =>
            {
                command.CommandText = "Select Name from Preferences where Category = 'Country';";
                var reader = command.ExecuteReader();
                var list = new List<String>();
                while (reader.Read())
                {
                    string preference = reader.GetString(0);
                    list.Add(preference);
                }
                return list;
            });
        }
        public List<String> GetType()
        {
            return RunCommand(command =>
            {
                command.CommandText = "Select Name from Preferences where Category = 'Type';";
                var reader = command.ExecuteReader();
                var list = new List<String>();
                while (reader.Read())
                {
                    string preference = reader.GetString(0);
                    list.Add(preference);
                }
                return list;
            });
        }

       /* public List<Package> GetAllPackages()
        {
            return RunCommand(command =>
                {
                    command.CommandText = "SELECT * FROM Packages";
                    var reader = command.ExecuteReader();
                    var list = new List<Package>();
                    while(reader.Read())
                    {
                        var pack = new Package();
                        pack.ID = reader.GetInt32(0);
                        pack.Name = reader.GetString(1);
                        pack.Type = reader.GetString(2);
                        pack.Included = reader.GetString();
                        pack.Transport = reader.GetString();
                        pack.Price = reader.GetDouble();
                        pack.SearchIndexRate = reader.GetDouble();
                        pack.BeginDate = reader.GetDateTime();
                        pack.EndDate = reader.GetDateTime();
                        pack.Picture = reader.GetBytes();
                        list.Add(pack);
                        
                    }
                    return list;
                });
        }*/


        public List<String> GetTypes()
        {
            return RunCommand(command =>
            {
                command.CommandText = " SELECT DISTINCT type FROM Package";
                var reader = command.ExecuteReader();
                var list = new List<String>();
                while (reader.Read())
                {
                    String t = reader.GetString(0);
                    list.Add(t);
                }
                return list;
            });
        }

    }
}
