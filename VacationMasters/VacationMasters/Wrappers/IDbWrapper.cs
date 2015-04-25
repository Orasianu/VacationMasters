﻿using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace VacationMasters.Wrappers
{
    public interface IDbWrapper
    {
        /// <summary>
        /// Get an opened connection
        /// </summary>
        /// <returns></returns>
        MySqlConnection GetConnection();

        /// <summary>
        /// Get a clear valid command
        /// </summary>
        /// <param name="connectionWrapper"></param>
        /// <returns></returns>
        MySqlCommand GetCommand(MySqlConnection connectionWrapper);

        /// <summary>
        /// Execute a valid SQL query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        T QueryValue<T>(string sqlQuery);
        List <string> GetPreferences();
        List <string> GetType();
    }
}
