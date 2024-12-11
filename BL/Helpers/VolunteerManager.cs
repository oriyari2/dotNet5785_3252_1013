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
using System.Net.Http.Headers;

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
              Distance = volunteer?.Address != null ? DistanceCalculator.CalculateDistance(GeocodingHelper.GetCoordinates(volunteer.Address), GeocodingHelper.GetCoordinates(call.Address)) : 0,

              status = (ClockManager.Now - call.MaxTimeToEnd) <= admin.GetRiskRange()
                      ? BO.Status.treatment
                      : BO.Status.riskTreatment
          }).First(); 

        return callInProgress;
    }


    internal static class GeocodingHelper
    {
        private static readonly HttpClient client = new HttpClient();

        static GeocodingHelper()
        {
            // הוספת User-Agent לבקשות HTTP
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GeocodingDistanceProject", "1.0"));
        }

        /// <summary>
        /// מקבלת כתובת ומחזירה קואורדינטות של קו רוחב וקו אורך.
        /// </summary>
        internal static (double Latitude, double Longitude) GetCoordinates(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Address must be a non-empty string");

            string baseUrl = "https://nominatim.openstreetmap.org/search";
            string requestUrl = $"{baseUrl}?q={Uri.EscapeDataString(address)}&format=json&addressdetails=1";

            try
            {
                var response = client.GetStringAsync(requestUrl).Result; // סינכרוני
                var data = JArray.Parse(response);

                if (data.Count == 0)
                    throw new Exception("No coordinates found for the provided address");

                double latitude = double.Parse(data[0]["lat"].ToString());
                double longitude = double.Parse(data[0]["lon"].ToString());

                return (latitude, longitude);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch coordinates: {ex.Message}");
            }
        }
    }

    internal static class DistanceCalculator
    {
        /// <summary>
        /// מחשבת את המרחק האווירי בין שתי נקודות קואורדינטות.
        /// </summary>
        internal static double CalculateDistance((double Latitude, double Longitude) point1, (double Latitude, double Longitude) point2)
        {
            const double EarthRadiusKm = 6371;

            double lat1Rad = DegreeToRadian(point1.Latitude);
            double lon1Rad = DegreeToRadian(point1.Longitude);
            double lat2Rad = DegreeToRadian(point2.Latitude);
            double lon2Rad = DegreeToRadian(point2.Longitude);

            double dlat = lat2Rad - lat1Rad;
            double dlon = lon2Rad - lon1Rad;

            double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Pow(Math.Sin(dlon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        /// <summary>
        /// ממירה מעלות לרדיאנים.
        /// </summary>
        internal static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
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



