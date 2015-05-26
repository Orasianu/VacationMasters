using System;
using System.Collections.Generic;
using VacationMasters.Essentials;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;
using System.Linq;


namespace VacationMasters.PackageManagement
{
    public class PackageManager
    {

        private readonly IDbWrapper _dbWrapper;
        private readonly IUserManager _userManager;

        public PackageManager(IDbWrapper dbWrapper)
        {
            _dbWrapper = dbWrapper;
            _userManager = new UserManager(_dbWrapper);
        }

        public List<Package> SearchPackages(string searchQuery)
        {
            List<Package> searchedPackages = new List<Package>();

            var packagesByName = _dbWrapper.GetPackagesByName(searchQuery);
            var packagesByType = _dbWrapper.getPackagesByType(searchQuery);

            if (packagesByName != null)
                searchedPackages.AddRange(packagesByName);

            if(packagesByType !=null)
                searchedPackages.AddRange(packagesByType);

            return searchedPackages;
        } 

        public void AddPackage(Package package)
        {
            var sql = string.Format("INSERT INTO Packages(Name, Type, Included, Transport, Price, SearchIndex, Rating,"
                                      + "BeginDate, EndDate, Picture,TotalVotes) "
                                      + "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}','0');",
                                      package.Name, package.Type, package.Included, package.Transport, package.Price,
                                      package.SearchIndex, package.Rating, package.BeginDate.ToString("yyyy-MM-dd HH:mm:ss"), package.EndDate.ToString("yyyy-MM-dd HH:mm:ss"), package.Picture);

            _dbWrapper.QueryValue<object>(sql);
        }
       
        public void Display(List<Package> l)
        {
            var list = l;


            foreach (Package pack in list)
            {


            }

        }

        public void RemovePackage(Package package)
        {
            var sql = string.Format("Delete From ChoosePackage Where IDPackage = {0}; DELETE FROM Packages WHERE ID = {0};  ", package.ID);
            _dbWrapper.QueryValue<object>(sql);
        }

        public List<Package> GetPackagesByPreferences()
        {
            User loggedUser = MainPage.CurrentUser;
            List<Package> packagesByPrefrences = new List<Package>();
            foreach (var pref in loggedUser.Preferences)
            {
                string sql = string.Format("Select * from packages where Type == {0}", pref.Category);
                List<Package> results = _dbWrapper.RunCommand(command =>
                {
                    command.CommandText = sql;
                    return _dbWrapper.ReadPackages(command);
                });

                foreach (var item in results)
                    packagesByPrefrences.Add(item);

                sql = string.Format("Select * from packages p join ChooseDestinations cd on (p.ID = cd.IDPackage)" +
                "join destinations on (cd.IDDestination = d.ID) where d.Name ={0};", pref.Category);
                results = _dbWrapper.RunCommand(command =>
                {
                    command.CommandText = sql;
                    return _dbWrapper.ReadPackages(command);
                });

                foreach (var item in results)
                    packagesByPrefrences.Add(item);
            }

            var sortedPackagesByPrefrences = packagesByPrefrences.OrderBy(p => p.Rating).ThenBy(p => p.SearchIndex).ToList<Package>();

            return sortedPackagesByPrefrences;
        }

       
        public List<Package> GetPackagesByHistoric()
        {
            User loggedUser = MainPage.CurrentUser;
            List<Package> packagesByHistoric = new List<Package>();

            string sql = "Select * from packages p join choosepackage cp on(p.ID = cp.IDPackage)" +
                "join Order o on (cp.IDOrder = o.ID)";

            List<Package> results = _dbWrapper.RunCommand(command =>
            {
                command.CommandText = sql;
                return _dbWrapper.ReadPackages(command);
            });

            var typeList = results.Select(p => p.Type);

            foreach(var res in results){
                sql = string.Format("Select Name from destinations d join ChooseDestinations cd on (cd.IDDestination = d.ID)" +
               "join packages p (p.ID = cd.IDPackage) where p.ID ={0};", res.ID);

                var results2 = _dbWrapper.RunCommand(command =>
                {
                
                    command.CommandText = sql;
                    return _dbWrapper.ReadPackages(command);
                });
                
                foreach(var dest in results2)
                    packagesByHistoric.Add(dest);
            }


            var nameList = packagesByHistoric.Select(p => p.Name);
            List<int> frequencyNameArray = new List<int>();
            foreach (var name in nameList)
                frequencyNameArray.Add(nameList.Count(nume => nume == name));
           
            List<int> frequencyTypeArray = new List<int>();
            foreach (var type in typeList)
                frequencyTypeArray.Add(typeList.Count(tip => tip == type));

            int index = 0;

            foreach (var i in frequencyTypeArray)
                if (i == frequencyTypeArray.Max())
                    index = frequencyTypeArray.IndexOf(i);

            var typeMax = frequencyTypeArray.ElementAt(index);

            foreach(var i in frequencyNameArray)
                if(i == frequencyNameArray.Max())
                    index = frequencyNameArray.IndexOf(i);

            var nameMax = frequencyNameArray.ElementAt(index);

            sql = string.Format("Select * from preferences d join ChooseDestinations cd on (cd.IDDestination = d.ID)" +
               "join packages p (p.ID = cd.IDPackage) where p.Type = {0} or d.Name = {1};", typeMax, nameMax);

            var selectedPackages = _dbWrapper.RunCommand(command =>
            {

                command.CommandText = sql;
                return _dbWrapper.ReadPackages(command);
            });

            List<Package> selected2Packages = new List<Package>();

            foreach (var item in selectedPackages)
                if (!results.Contains(item))
                    selected2Packages.Add(item);

            selected2Packages.OrderBy(p => p.Rating).ThenBy(p => p.SearchIndex);

            return selected2Packages;

        }


        public List<Package> GetPackagesByUserGroups()
        {
            User loggedUser = MainPage.CurrentUser;
            var packagesByUserGroups = new List<Package>();

            var sql = string.Format("Select Name from Groups g join ChooseGroups cg on g.ID = cg.IDGroup" +
                                    "join Users u on u.ID = cg.IDUser where u.UserName = {0}", loggedUser.UserName);

            var groups = new List<String>();
            groups = _userManager.GetStrings(sql);
            var users = new List<String>();

            foreach (var group in groups)
            {
                sql = string.Format("Select ID from Users u join ChooseGroups cg on u.ID = cg.IDUser" +
                                    "join Groups g on g.ID = cg.IDGroup where Name ={0}", group);
                var temp = new List<String>();
                temp = _userManager.GetStrings(sql);
                users.AddRange(temp);
            }

            foreach (var user in users)
            {
                sql = string.Format("Select * from Packages p join ChoosePackage cp on p.ID = cp.IDPackage" +
                                    "join Order o on o.ID = cp.IDOrder join Users u on " +
                                    "u.ID = o.IDUser where u.ID = {0}", user);

                var temp = new List<Package>();

                temp =  _dbWrapper.RunCommand(command =>
                {
                    command.CommandText = sql;
                    return _dbWrapper.ReadPackages(command);
                });

                packagesByUserGroups.AddRange(temp);
            }

            var noduplicatespackagesByUserGroups = packagesByUserGroups.Distinct();
            var finalList = GetPackagesByPreferences();

            finalList.AddRange(noduplicatespackagesByUserGroups);

            finalList.OrderBy(p => p.Rating).ThenBy(p => p.SearchIndex);

            return finalList;

        }
        public int CheckIfUserHasOrderedThePackage(int packageId, string userName)
        {
            var sql = string.Format("SELECT o.Id FROM Users u JOIN  Orders o  ON  u.ID = o.IDUser JOIN ChoosePackage c ON o.ID = c.IDOrder Where c.IDPackage = {0} AND  u.UserName= '{1}' ", packageId, userName);
            return _dbWrapper.QueryValue<int>(sql);
        }
        public bool CheckIfUserDidVote(int packageId, string userName)
        {
            var sql = string.Format("SELECT c.HasRated FROM Users u JOIN  Orders o  ON  u.ID = o.IDUser JOIN ChoosePackage c ON o.ID = c.IDOrder Where c.IDPackage = {0} AND  u.UserName= '{1}' ", packageId, userName);
            return _dbWrapper.QueryValue<bool>(sql);
        }
        public int RetrieveUserId(string userName)
        {
            var sql = string.Format("Select ID From Users  Where UserName = '{0}';", userName);
            return _dbWrapper.QueryValue<int>(sql);
        }
        public void ReservePackage(string userName, DateTime now, double price, int id)
        {
            var userId = RetrieveUserId(userName);
            var sql = string.Format("INSERT INTO Orders(IDUser,Status, Date , TotalPrice)" +
                        "values('{0}', '{1}', '{2}', '{3}');", userId, "Reserved", now.ToString("yyyy-MM-dd HH:mm:ss"), price);
            sql += "SELECT LAST_INSERT_ID();";
            var idOrder = _dbWrapper.QueryValue<int>(sql);
            sql = string.Format("INSERT INTO ChoosePackage(IDOrder,IDPackage,HasRated) values('{0}','{1}','0');", idOrder, id);
            _dbWrapper.QueryValue<object>(sql);
        }

        public void CancelReservation(int packageId, int orderId)
        {
            var sql = string.Format("DELETE FROM ChoosePackage" +
                        " WHERE IDPackage = {0} AND IDOrder = {1};", packageId, orderId);
            sql += string.Format("Delete from Orders where ID = '{0}';", orderId);
            _dbWrapper.QueryValue<object>(sql);
        }

        public void UpdateRating(int packageId, double rating, int orderId)
        {
            var sql = string.Format("UPDATE Packages" + " SET Rating = (Rating*TotalVotes+{1})/(TotalVotes+1)" +
            " WHERE Id = {0};"+" UPDATE Packages SET TotalVotes = TotalVotes + 1  Where Id={0}; "+ "UPDATE ChoosePackage SET HasRated = 1 Where IDPackage = {0} And IDOrder = {2}; ", packageId, rating,orderId);
            _dbWrapper.QueryValue<object>(sql);
        }
       
    }
}
