using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MJAroundTheWorld.Controllers
{
    public class PhotoController : Controller
    {
        private readonly Dictionary<string, string> _stateLookup = new Dictionary<string, string>();

        public PhotoController()
        {
            // Add all the default US states for name matching:
            _stateLookup.Add("AE", "Armed Forces Europe");
            _stateLookup.Add("AL", "Alabama");
            _stateLookup.Add("AK", "Alaska");
            _stateLookup.Add("AZ", "Arizona");
            _stateLookup.Add("AR", "Arkansas");
            _stateLookup.Add("CA", "California");
            _stateLookup.Add("CO", "Colorado");
            _stateLookup.Add("CT", "Connecticut");
            _stateLookup.Add("DE", "Delaware");
            _stateLookup.Add("DC", "District of Columbia");
            _stateLookup.Add("FL", "Florida");
            _stateLookup.Add("GA", "Georgia");
            _stateLookup.Add("HI", "Hawaii");
            _stateLookup.Add("ID", "Idaho");
            _stateLookup.Add("IL", "Illinois");
            _stateLookup.Add("IN", "Indiana");
            _stateLookup.Add("IA", "Iowa");
            _stateLookup.Add("KS", "Kansas");
            _stateLookup.Add("KY", "Kentucky");
            _stateLookup.Add("LA", "Louisiana");
            _stateLookup.Add("ME", "Maine");
            _stateLookup.Add("MD", "Maryland");
            _stateLookup.Add("MA", "Massachusetts");
            _stateLookup.Add("MI", "Michigan");
            _stateLookup.Add("MN", "Minnesota");
            _stateLookup.Add("MS", "Mississippi");
            _stateLookup.Add("MO", "Missouri");
            _stateLookup.Add("MT", "Montana");
            _stateLookup.Add("NE", "Nebraska");
            _stateLookup.Add("NV", "Nevada");
            _stateLookup.Add("NH", "New Hampshire");
            _stateLookup.Add("NJ", "New Jersey");
            _stateLookup.Add("NM", "New Mexico");
            _stateLookup.Add("NY", "New York");
            _stateLookup.Add("NC", "North Carolina");
            _stateLookup.Add("ND", "North Dakota");
            _stateLookup.Add("OH", "Ohio");
            _stateLookup.Add("OK", "Oklahoma");
            _stateLookup.Add("OR", "Oregon");
            _stateLookup.Add("PA", "Pennsylvania");
            _stateLookup.Add("RI", "Rhode Island");
            _stateLookup.Add("SC", "South Carolina");
            _stateLookup.Add("SD", "South Dakota");
            _stateLookup.Add("TN", "Tennessee");
            _stateLookup.Add("TX", "Texas");
            _stateLookup.Add("UT", "Utah");
            _stateLookup.Add("VT", "Vermont");
            _stateLookup.Add("VA", "Virginia");
            _stateLookup.Add("WA", "Washington");
            _stateLookup.Add("WV", "West Virginia");
            _stateLookup.Add("WI", "Wisconsin");
            _stateLookup.Add("WY", "Wyoming");
            _stateLookup.Add("AS", "American Samoa");
            _stateLookup.Add("GU", "Guam");
            _stateLookup.Add("MH", "Marshall Islands");
            _stateLookup.Add("FM", "Micronesia");
            _stateLookup.Add("MP", "Northern Mariana Islands");
            _stateLookup.Add("PW", "Palau");
            _stateLookup.Add("PR", "Puerto Rico");
            _stateLookup.Add("VI", "U.S. Virgin Islands");
        }

        public ActionResult Index(int? id)
        {
            ViewBag.ShowImageOnly = !id.HasValue;

            if (!ViewBag.ShowImageOnly)
            {
                using (var ent = new LocationImagesEntities())
                {
                    var location = ent.Locations.FirstOrDefault(l => l.LocationId == id);
                    ViewBag.Location = location ?? throw new HttpException(404, "Location not found using ID: " + id);
                    ViewBag.Photos = location.Photos.Where(p=>p.PhotoTakenDate.Value.Year == DateTime.Now.Year).OrderByDescending(p => p.RecordCreatedDateTime).ToList();
                }
            }

            return View();
        }

        string[] ValidExtensions = { "jpg", "jpeg", "png", "gif", "bmp" };


        [ValidateInput(false)]
        public ActionResult Upload() 
        {
            // First, get all the location "states"
            using (var ent = new LocationImagesEntities())
            {
                var states = ent.Locations.Select(loc => loc.State).Distinct();

                var stateList = new List<SelectListItem>();


                var textInfo = new CultureInfo("en-US", false).TextInfo;

                foreach (var locationState in states)
                {
                    if (_stateLookup.ContainsKey(locationState))
                    {
                        stateList.Add(new SelectListItem
                        {
                            Text = _stateLookup[locationState],
                            Value = locationState
                        });
                    }
                    else
                    {
                        stateList.Add(new SelectListItem
                        {
                            Text = textInfo.ToTitleCase(locationState.ToLower()),
                            Value = locationState
                        });
                    }
                }

                ViewBag.StateList = stateList.OrderBy(sl => sl.Text);
            }

            if (Request.Files.Count > 0 && Request.Files[0] != null)
            {
                if (IsValid())
                {
                    var file = Request.Files[0];
                    using (var ent = new LocationImagesEntities())
                    {
                        var city = Request.Form["photoTakenCity"].Trim();
                        var state = Request.Form["photoTakenState"].Trim();

                        var foundLocation = ent.Locations.FirstOrDefault(l => l.City == city && l.State == state);

                        if (foundLocation == null)
                        {
                            string url = String.Format("http://nominatim.openstreetmap.org/search?city={0}&{1}={2}&format=json", Server.UrlEncode(city), _stateLookup.ContainsValue(state.ToUpper()) ? "state" : "country", Server.UrlEncode(state));
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
                            request.Referer = "http://www.microsoft.com";
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                                    {
                                        List<GeoCode> gcs = JsonConvert.DeserializeObject<List<GeoCode>>(reader.ReadToEnd());

                                        if (gcs.Count > 0)
                                        {
                                            foundLocation = ent.Locations.Add(new Location() { City = city.ToUpper(), Latitude = Double.Parse(gcs.First().lat), Longitude = Double.Parse(gcs.First().lon), State = state.ToUpper() });
                                            ent.SaveChanges();
                                        }
                                        else
                                            throw new HttpException(404, "City not found");
                                    }
                                }
                                else
                                    throw new HttpException(400, response.StatusDescription);
                            }
                        }

                        var newPhoto = new Photo
                        {
                            LocationId = foundLocation.LocationId,
                            FileName = file.FileName.Split('\\').Last(),
                            Description = string.IsNullOrWhiteSpace(Request.Form["description"]) ? null : Request.Form["description"].Trim(),
                            ImageProvidedBy = string.IsNullOrWhiteSpace(Request.Form["photoSubmittedBy"]) ? null : Request.Form["photoSubmittedBy"],
                            PhotoTakenDate = string.IsNullOrWhiteSpace(Request.Form["photoTakenDate"]) ? null : (DateTime?)DateTime.Parse(Request.Form["photoTakenDate"]),
                            RecordCreatedDateTime = DateTime.Now
                        };

                        newPhoto.PhotoImage = new PhotoImage
                        {
                            Image = ReadFully(file.InputStream)
                        };

                        ent.Photos.Add(newPhoto);

                        ent.SaveChanges();
                    }

                    ViewBag.Message = "<span class=\"complete\">Photo successfully uploaded!</span><br /><br />";
                }
            }

            return View();
        }

        public ActionResult Photo(int photoId)
        {
            PhotoImage foundPhoto;
            Photo photo;

            using (var ent = new LocationImagesEntities())
            {
                foundPhoto = ent.PhotoImages.FirstOrDefault(p => p.PhotoId == photoId);
                photo = foundPhoto.Photo;
            }

            if (foundPhoto == null)
            {
                throw new HttpException(404, "Photo not found using ID: " + photoId);
            }
            else
            {
                Response.AppendHeader("Content-Disposition", "inline; filename=" + photo.FileName);
                return File(foundPhoto.Image, MimeMapping.GetMimeMapping(photo.FileName));
            }
        }

        private bool IsValid()
        {
            var file = Request.Files[0];

            var ext = file.FileName.Split('.')[file.FileName.Split('.').Length - 1].ToLower();

            if (file.ContentLength == 0 || !ValidExtensions.Contains(ext))
            {
                ViewBag.Message = "<span class=\"required\">Please upload a valid image file!</span><br /><br />";
                return false;
            }

            DateTime takenDate;
            if (!string.IsNullOrWhiteSpace(Request.Form["photoTakenDate"]) && !DateTime.TryParse(Request.Form["photoTakenDate"], out takenDate))
            {
                ViewBag.Message = "<span class=\"required\">Invalid photo taken date!</span><br /><br />";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Request.Form["photoTakenCity"]))
            {
                ViewBag.Message = "<span class=\"required\">City name must be entered!</span><br /><br />";
            }

            return true;
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

    [DataContract]
    public class Address
    {
        [DataMember]
        public string road { get; set; }
        [DataMember]
        public string suburb { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string state_district { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string postcode { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string country_code { get; set; }
    }

    [DataContract]
    public class GeoCode
    {
        [DataMember]
        public string place_id { get; set; }
        [DataMember]
        public string licence { get; set; }
        [DataMember]
        public string osm_type { get; set; }
        [DataMember]
        public string osm_id { get; set; }
        [DataMember]
        public string lat { get; set; }
        [DataMember]
        public string lon { get; set; }
        [DataMember]
        public string display_name { get; set; }
        [DataMember]
        public Address address { get; set; }
    }
}