namespace DalTest;

using Dal;
using DalApi;
using DO;
using System;
using System.Data;

public static class Initialization
{
    private static IAssignment? s_dalAssignment; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();

    private static void createAssignment()
    {

    }

    private static void createCall()
    {

    }

    private static void createVolunteer()
    {  
        string[] VolNames ={ "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin",
         "Dina Klein", "Shira Israelof","yael cohen", "david azrad", "miri levi", "amir shalom",
        "ronit shahar", "itay ben-david", "maya peretz", "yoni abergil", "lior haddad","rivka azrad" }; //potential names

        string?[] VolAdresses = {"Jerusalem, King George St 1, Israel", "Jerusalem, Jaffa St 23, Israel",
        "Jerusalem, Agripas St 45, Israel","Jerusalem, Ben Yehuda St 67, Israel", "Jerusalem, HaNeviim St 89, Israel",
        "Jerusalem, Yad Harutsim St 101, Israel","Jerusalem, Emek Refaim St 15, Israel",
        "Jerusalem, Malha Rd 33, Israel", "Jerusalem, Harav Kook St 2, Israel", "Jerusalem, Hillel St 40, Israel",
        "Jerusalem, Davidka Square 5, Israel", "Jerusalem, Ramban St 21, Israel",
        "Jerusalem, Tchernichovsky St 37, Israel", "Jerusalem, HaPalmach St 11, Israel",
        "Jerusalem, Keren HaYesod St 90, Israel","Jerusalem, Hapisga St 30, Israel"}; //potential adresses

        double?[] VolLatitudes = {31.776, 31.771, 31.777, 31.779, 31.783, 31.791,  31.758,
        31.750, 31.768, 31.778, 31.761, 31.773,  31.759, 31.780, 31.762,31.78};
        //latitudes corresponding to the list of addresses

        double?[] VolLongitudes = {35.2218, 35.2137, 35.2212, 35.2135, 35.2143, 35.2007, 35.2072,
        35.1907, 35.2091, 35.2181, 35.2133, 35.2046,35.2150, 35.1984, 35.2075,35.21};
        //longitudes corresponding to the list of addresses

        for (int i = 0; i < 16; i++) //create 15 volunteers and 1 manager
        {
            int VolId;
            do
                VolId = s_rand.Next(200000001, 400000000);
            while (s_dalVolunteer!.Read(VolId) != null); //until find available id

            RoleType VulRole = (i == 0) ? RoleType.manager : RoleType.volunteer; //1 manager 15 volunteers

            string VolPhone = "05" + s_rand.Next(10000000, 99999999).ToString(); //random valid phone number

            string VolEmail = VolPhone + "@email.com"; //valid email with the volunteer phone number

            double VolMaxDis = s_rand.Next(); //random distance

            s_dalVolunteer!.Create(new(VolId, VolNames[i], VolPhone, VolEmail, null, VolAdresses[i],
            VolLatitudes[i], VolLongitudes[i], VulRole, true, VolMaxDis, DistanceType.air)); 
            //add new Volunteer with right details to list
        }
    }

}
