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
        // קבלת קווי אורך ורוחב לשתי הכתובות
        GetCoordinatesFromAddress(address1, out double lat1, out double lon1);
        GetCoordinatesFromAddress(address2, out double lat2, out double lon2);

        // חישוב מרחק בעזרת נוסחת Haversine
        double R = 6371; // רדיוס כדור הארץ בקילומטרים
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // המרחק בקילומטרים
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }



    internal static void GetCoordinatesFromAddress(string address, out double latitude, out double longitude)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("The address provided is invalid or empty.");
        }

        string baseUrl = "https://nominatim.openstreetmap.org/search";
        string url = $"{baseUrl}?q={Uri.EscapeDataString(address)}&format=json&limit=1";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; DistanceCalculator/1.0)");

            HttpResponseMessage response = client.GetAsync(url).Result; // קריאה סינכרונית
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch coordinates. HTTP Status: {response.StatusCode}");
            }

            string responseData = response.Content.ReadAsStringAsync().Result;
            var locationData = JsonSerializer.Deserialize<dynamic[]>(responseData);

            if (locationData == null || locationData.Length == 0)
            {
                throw new Exception("The address provided is invalid or could not be found.");
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



