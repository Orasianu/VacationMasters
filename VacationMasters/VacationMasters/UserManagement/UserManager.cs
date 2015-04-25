using System;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using VacationMasters.Essentials;
using VacationMasters.Wrappers;
using System.Collections.Generic;

namespace VacationMasters.UserManagement
{
    public class UserManager : IUserManager
    {
        private readonly IDbWrapper _dbWrapper;

        public UserManager(IDbWrapper dbWrapper)
        {
            _dbWrapper = dbWrapper;
        }

        public bool CanLogin(User user)
        {
            //TODO: check for existing user
            return !user.Banned;
        }

        public bool CheckCredentials(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string userName, string newPassword)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string userName)
        {
            var sql = string.Format("SELECT UserName, FirstName, LastName, Email, PhoneNumber, " +
                                    "Banned, Type, KeyWordsSearches from users " +
                                    "where UserName = '{0}';", userName);
            User user = null;

            return _dbWrapper.RunCommand(command =>
            {
                command.CommandText = sql;
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    userName = reader.GetString(0);
                    var firstName = reader.GetString(1);
                    var lastName = reader.GetString(2);
                    var email = reader.GetString(3);
                    var phoneNumber = reader.GetString(4);
                    var banned = reader.GetBoolean(5);
                    var type = reader.GetString(6);
                    var keyWordsSearchers = reader.GetString(7);
                    user = new User(userName, firstName, lastName, email, phoneNumber, banned,
                        type, keyWordsSearchers);
                }
                return user;
            });
        }

        public void AddUser(User user, string password, string type = "User")
        {
            var input = CryptographicBuffer.ConvertStringToBinary(password,
            BinaryStringEncoding.Utf8);
            var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA256");
            var hashed = hasher.HashData(input);
            var pwd = CryptographicBuffer.EncodeToBase64String(hashed);
            var sql = string.Format("INSERT INTO Users(UserName, FirstName, LastName, Email, PhoneNumber," +
            "Password, Banned, Type, KeyWordsSearches) " +
            "values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}');",
            user.UserName, user.FirstName, user.LastName, user.Email, user.PhoneNumber, pwd,
            false, type, user.KeyWordSearches);
            _dbWrapper.QueryValue<object>(sql);
        }
        public void UpdateUser(string user, bool newsletter, string email, string password, string password_confirm, string PreferencesCountry, string PreferencesType)
        {
            var input = CryptographicBuffer.ConvertStringToBinary(password,
            BinaryStringEncoding.Utf8);
            var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA256");
            var hashed = hasher.HashData(input);
            var pwd = CryptographicBuffer.EncodeToBase64String(hashed);
            if (email != null && password == null && newsletter == false && PreferencesCountry == null && PreferencesType == null)
            {
                var sql = string.Format("Update Users set Email = {0}", email);
                _dbWrapper.QueryValue<object>(sql);
            }
            if (email == null && password != null && newsletter == false && PreferencesCountry == null && PreferencesType == null)
            {
                if (password == password_confirm)
                {
                    var sql = string.Format("Update Users set Password = {0}", pwd);
                    _dbWrapper.QueryValue<object>(sql);
                }
            }
            if (email == null && password == null && newsletter == false && PreferencesCountry != null && PreferencesType == null)
            {
                var sql = string.Format("Select ID from User where UserName = {0}", user);
                var idUser = _dbWrapper.QueryValue<object>(sql);
                var sql1 = string.Format("Select ID from Preferences where name = {0} and Category = 'Country'", PreferencesCountry);
                var idPreferences = _dbWrapper.QueryValue<object>(sql);
                var sql2 = string.Format("INSERT INTO ChoosePreferences(IDUser,IDPreference) values('{0}','{1}');", idUser, idPreferences);

            }
            if (email == null && password == null && newsletter == false && PreferencesCountry == null && PreferencesType != null)
            {
                var sql = string.Format("Select ID from User where UserName = {0}", user);
                var idUser = _dbWrapper.QueryValue<object>(sql);
                var sql1 = string.Format("Select ID from Preferences where name = {0} and Category = 'Type'", PreferencesType);
                var idPreferences = _dbWrapper.QueryValue<object>(sql);
                var sql2 = string.Format("INSERT INTO ChoosePreferences(IDUser,IDPreference) values('{0}','{1}');", idUser, idPreferences);

            }
            if( email == null && password == null && newsletter == true && PreferencesCountry == null && PreferencesType == null)
            {
                    // update newsletter
            }
        }

        public bool CheckIfUserExists(string userName)
        {
            var sql = string.Format("Select ID From Users Where UserName = {0};", userName);
            if (_dbWrapper.QueryValue<int>(sql) == 0) return false;
            return true;
        }
        public bool CheckIfEmailExists(string email)
        {
            var sql = string.Format("Select ID From Users Where Email = {0};", email);
            if (_dbWrapper.QueryValue<int>(sql) == 0) return false;
            return true;
        }
        public void AddUser(User user, string password, List<int> preferencesIds, string type = "User")
        {
            var input = CryptographicBuffer.ConvertStringToBinary(password,
           BinaryStringEncoding.Utf8);
            var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA256");
            var hashed = hasher.HashData(input);
            var pwd = CryptographicBuffer.EncodeToBase64String(hashed);
            var sql = string.Format("INSERT INTO Users(UserName, FirstName, LastName, Email, PhoneNumber," +
                                    "Password, Banned, Type, KeyWordsSearches) " +
                                    "values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}');",
                user.UserName, user.FirstName, user.LastName, user.Email, user.PhoneNumber, pwd,
                false, type, user.KeyWordSearches);
            sql += "SELECT LAST_INSERT_ID();";
            var idUser = _dbWrapper.QueryValue<int>(sql);
            foreach (var id in preferencesIds)
            {
                sql = string.Format("INSERT INTO ChoosePreferences(IDUser,IDPreference) values('{0}','{1}');", idUser, id);
            }
            _dbWrapper.QueryValue<object>(sql);
        }

        public void AddPreference(Preference preference)
        {
            var sql = string.Format("INSERT INTO Preferences(Name,Category) values ('{0}','{1}');", preference.Name, preference.Category);
            _dbWrapper.QueryValue<object>(sql);
        }

        public void RemoveUser(string userName)
        {
            var sql = string.Format("DELETE FROM ChoosePreferences" +
                                    " WHERE IDUser = (SELECT ID FROM Users Where UserName = {0});",
                                    userName);
            sql += string.Format("Delete from Users where UserName = '{0}';", userName);
            _dbWrapper.QueryValue<object>(sql);
        }

        public void BanUser(string userName)
        {
            var sql = string.Format("UPDATE Users set Banned = true " +
                                    "WHERE UserName = '{0}';",userName);
            _dbWrapper.QueryValue<object>(sql);
        }

        public void UnbanUser(string userName)
        {
            var sql = string.Format("UPDATE Users set Banned = false " +
                                   "WHERE UserName = '{0}';", userName);
            _dbWrapper.QueryValue<object>(sql);
        }
    }
}
