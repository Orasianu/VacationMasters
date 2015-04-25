<<<<<<< HEAD
﻿using MySql.Data.MySqlClient;
using System.Collections.Generic;
=======
﻿using System;
using MySql.Data.MySqlClient;
>>>>>>> e5658eda3422d633bcb623984238bab42fee6ef3

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
<<<<<<< HEAD
        List <string> GetPreferences();
        List <string> GetType();
=======

        /// <summary>
        /// Run a sql command
        /// </summary>
        /// <param name="func"></param>
        T RunCommand<T>(Func<MySqlCommand, T> func);
>>>>>>> e5658eda3422d633bcb623984238bab42fee6ef3
    }
}
