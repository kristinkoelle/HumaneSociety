﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    
    public static class Query
    {
        public delegate Employee employeeDelegate(HumaneSocietyDataContext db, Employee employee);
        static public Employee RunEmployeeQueries(Employee employee, string crud)
        {
            employeeDelegate employeeDelegate;
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

                switch (crud)
                {
                    case "create":
                        employeeDelegate = Create;
                        employeeDelegate(db, employee);
                        break;
                    case "read":
                        employeeDelegate = Read;
                        employee = employeeDelegate(db, employee);
                    break;
                    case "update":
                        employeeDelegate = Update;
                        employeeDelegate(db, employee);
                        break;
                    case "delete":
                        employeeDelegate = Delete;
                        employeeDelegate(db, employee);
                    break;
                }

            db.SubmitChanges();
            return employee;
        }
        public static Employee Create(HumaneSocietyDataContext db, Employee employee)
        {
            db.Employees.InsertOnSubmit(employee);
            return employee;
        }
        public static Employee Read(HumaneSocietyDataContext db, Employee employee)
        {
            var employeeSearch = (
                from employeeObject in db.Employees
                where employee.employeeNumber == employeeObject.employeeNumber
                select employeeObject).ToList();
            return employeeSearch[0];
        }
        public static Employee Update(HumaneSocietyDataContext db, Employee employee)
        {
            var employeeSearch = (
                from employeeObject in db.Employees
                where employee.ID == employeeObject.ID
                select employeeObject).ToList();
            return employeeSearch[0];
        }
        public static Employee Delete(HumaneSocietyDataContext db, Employee employee)
        {
            var employeeSearch = (
                from employeeObject in db.Employees
                where employee.employeeNumber == employeeObject.employeeNumber && employee.lastName == employeeObject.lastName
                select employeeObject).ToList();
            db.Employees.DeleteOnSubmit(employeeSearch[0]);
            return employee;
        }

        public static Client GetClient(string username, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var client = (from user in db.Clients where user.userName == username && user.pass == password select user).ToList();
            return client[0];
        }

        public static IEnumerable<ClientAnimalJunction> GetUserAdoptionStatus(Client client) // this is the format we need to be using
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            //get client return adoption
            var approvalStatus = (
                from junction in db.ClientAnimalJunctions
                where junction.client == client.ID
                select junction
                );
                return approvalStatus;
        }

        public static Animal GetAnimalByID(int iD)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            //search query for ID return animal object
             var animalObject = (
                from animal in db.Animals
                where iD == animal.ID
                select animal
                ).ToList();
            return animalObject[0];
        }

        public static void Adopt(Animal animal, Client client) 
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            //search for animal, search for client, assign animal to client, change adopted status on animal to pending approval
            var newApplicant = (
                from newlyApplied in db.ClientAnimalJunctions
                where client.ID == newlyApplied.Client1.ID
                    && animal.ID == newlyApplied.Animal1.ID
                select newlyApplied
                );
            foreach(ClientAnimalJunction newlyAdopted in newApplicant)
            {
                newlyAdopted.approvalStatus = "pending";
            }
            db.SubmitChanges();
        }

        public static IQueryable<Client> RetrieveClients()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            //return //list of clients
            var clientQuery = (
                from allClients in db.Clients
                select allClients
                );
            return clientQuery;
        }

        public static IQueryable<USState> GetStates()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            //return list of all states
            var allStates = (
                from name in db.USStates
                select name
                );
            return allStates;
        }

        public static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateID)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Client client = new Client();
            client.firstName = firstName;
            client.lastName = lastName;
            client.userName = username;
            client.pass = password;
            client.email = email;
            UserAddress address = new UserAddress();
            address.addessLine1 = streetAddress;
            address.zipcode = zipCode;
            var userState = (
                from state in db.USStates
                where state.ID == stateID
                select state
                ).ToList();                
            address.USState = userState[0];
            db.UserAddresses.InsertOnSubmit(address);
            db.SubmitChanges();
            var userAddress = (
                from addresses in db.UserAddresses
                where streetAddress == address.addessLine1
                select address).ToList();
            client.userAddress = userAddress[0].ID;
            db.Clients.InsertOnSubmit(client);
            db.SubmitChanges();
            
        }

        internal static void UpdateClient(Client client)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var searchClients = (
                from searchClient in db.Clients
                where searchClient.ID == client.ID
                select searchClient);
            foreach (Client searchClient in searchClients)
            {
                searchClient.email = client.email;
                searchClient.firstName = client.firstName;
                searchClient.homeSize = client.homeSize;
                searchClient.income = client.income;
                searchClient.kids = client.kids;
                searchClient.lastName = client.lastName;
                searchClient.pass = client.pass;
                searchClient.userAddress = client.userAddress;
                searchClient.userName = client.userName;
            }
            
            db.SubmitChanges();
            Console.WriteLine("Changes Submitted");
        }
               
        internal static IEnumerable<ClientAnimalJunction> GetPendingAdoptions() 
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            //search for animal, search for client, assign animal to client, change adopted status on animal to pending approval

            var pendingApplicant = (
                from newlyApplied in db.ClientAnimalJunctions
                where newlyApplied.approvalStatus == "pending" 
                select newlyApplied
                );
            return pendingApplicant;
        }

        internal static void UpdateAdoption(bool v, ClientAnimalJunction clientAnimalJunction)
        {
            throw new NotImplementedException();
        }

        public static IQueryable<AnimalShotJunction> GetShots(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalID =(
                from animalSearch in db.AnimalShotJunctions
                where animal.ID == animalSearch.Animal_ID
                select animalSearch
                );
            return animalID;
        }

        public static IQueryable<int> FindShotId(string v)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var shotId = (
                from shots in db.Shots
                where v.ToLower() == shots.name.ToLower()
                select shots.ID
                );
            return shotId;
        }

        internal static void UpdateShot(string v, Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var shotUpdate = (
                from update in db.AnimalShotJunctions
                where animal.ID == update.Animal_ID //&& update.Shot_ID == shotId
                select update
                );
            foreach(AnimalShotJunction updates in shotUpdate)
            {
                updates.dateRecieved = DateTime.Now;
            }
        }

        internal static void EnterUpdate(Animal animal, Dictionary<int, string> updates)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static int? GetBreed()
        {
            throw new NotImplementedException();
        }

        internal static int? GetDiet()
        {
            throw new NotImplementedException();
        }

        internal static int? GetLocation()
        {
            throw new NotImplementedException();
        }

        internal static void AddAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employee = (
                from validEmployee in db.Employees
                where validEmployee.userName == userName && validEmployee.pass == password
                select validEmployee
            ).ToList();
            return employee[0];
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employee = (
                from emp in db.Employees
                where emp.email == email && emp.employeeNumber == employeeNumber
                select emp
            ).ToList();
            return employee[0];
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var updateLogin = (
                from employeeToChange in db.Employees
                where employeeToChange.ID == employee.ID
                select employeeToChange);
            foreach (Employee employeeToChange in updateLogin)
            {
                employeeToChange.userName = employee.userName;
                employeeToChange.pass = employee.pass;
            }
            db.SubmitChanges();
        }


        internal static bool CheckEmployeeUserNameExist(string username)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var doesUserExist = (
                from validUsers in db.Employees
                where username == validUsers.userName
                select validUsers
                ).ToList();
            if (doesUserExist.Count == 0) { return false; }
            else { return true; }
        }
    }
}
