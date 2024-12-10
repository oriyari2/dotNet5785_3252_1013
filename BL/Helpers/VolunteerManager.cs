using DalApi;
using Microsoft.VisualBasic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using BlApi;
using BlImplementation;
using System.Data.Common;
using System.Xml.Linq;
using System.Collections.Generic;


namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get;
    internal static int TotalCall(int id, DO.EndType endType) =>s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.TheEndType == endType)).Count();
    internal static BO.CallInProgress callProgress(int id)
    {
        var assignments = s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.TheEndType == null));
        var volunteer = s_dal.Volunteer.Read(id);
        AdminImplementation admin =new();
        BO.CallInProgress callInProgress =
         (from item in assignments
          let call = s_dal.Call.Read(item.CallId)
          select new BO.CallInProgress
          {
              Id = item.Id,
              CallId = item.CallId,
              TheCallType = (BO.CallType)call.TheCallType,
              VerbalDescription = call.VerbalDescription,
              Address = call.Address,
              OpeningTime = call.OpeningTime,
              MaxTimeToEnd = call.MaxTimeToEnd,
              EntryTime = item.EntryTime,
              Distance = volunteer?.Address != null ? GetDistance(volunteer.Address, call.Address) : 0,

              status = (ClockManager.Now - call.MaxTimeToEnd) <= admin.GetRiskRange()
                      ? BO.Status.treatment
                      : BO.Status.riskTreatment
          }).First(); 

        return callInProgress;
    }

    
    internal static double GetDistance(string address1, string address2)
    {
        string apiKey = "AIzaSyC0zQlfOYgjYpr12DVxTc6Sw8u6lqzUX2U"; // הכניסי כאן את מפתח ה-API שלך
        string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(address1)}&destinations={Uri.EscapeDataString(address2)}&key={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;

                var json = JObject.Parse(jsonResponse);

                var distanceInMeters = json["rows"]?[0]?["elements"]?[0]?["distance"]?["value"]?.ToObject<double>();
                if (distanceInMeters.HasValue)
                    return distanceInMeters.Value / 1000; 
            }

            throw new OurSystemException("Failed to retrieve distance. Please check your API key and addresses.");
        }
    }


    internal static void GetCoordinatesFromAddress(string address, out double latitude, out double longitude)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("The address provided is invalid or empty.");
        }

        const string apiKey = "AIzaSyC0zQlfOYgjYpr12DVxTc6Sw8u6lqzUX2U"; // יש להחליף במפתח API תקף
        string baseUrl = "https://us1.locationiq.com/v1/search.php";
        string url = $"{baseUrl}?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(url).Result; // קריאה סינכרונית
            if (!response.IsSuccessStatusCode)
            {
                throw new OurSystemException($"Failed to fetch coordinates. HTTP Status: {response.StatusCode}");
            }

            string responseData = response.Content.ReadAsStringAsync().Result;
            var locationData = JsonSerializer.Deserialize<dynamic[]>(responseData);

            if (locationData == null || locationData.Length == 0)
            {
                throw new OurSystemException("The address provided is invalid or could not be found.");
            }

            // קו רוחב וקו אורך
            latitude = double.Parse(locationData[0]["lat"].ToString());
            longitude = double.Parse(locationData[0]["lon"].ToString());
        }
    }

    internal static  IEnumerable<BO.Volunteer> ReadAll(bool? active, BO.FieldsVolunteerInList field = BO.FieldsVolunteerInList.Id)
    { 
    var listVol = s_dal.Volunteer.ReadAll(s => s.Active == active);
   var listSort = from item in listVol
                  orderby field//We need to change to switch
                  select item;
    return (IEnumerable < BO.Volunteer >) listVol;
    }

};



