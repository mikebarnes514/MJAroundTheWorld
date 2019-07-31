using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace MJAroundTheWorld
{
    public partial class MapViewer : System.Web.UI.Page
    {
        private readonly Dictionary<string, string> _stateLookup = new Dictionary<string, string>();

        public MapViewer()
        {
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

        protected void Page_Load(object sender, EventArgs e)
        {
            using (var ent = new LocationImagesEntities())
            {
                var result = ent.GetAllUsedCityStates();
                var states = ent.Locations.Select(loc => loc.State).Distinct();
                var stateList = new List<ListItem>();
                var textInfo = new CultureInfo("en-US", false).TextInfo;

                foreach (var locationState in states)
                {
                    if (_stateLookup.ContainsKey(locationState))
                        stateList.Add(new ListItem(_stateLookup[locationState], locationState));
                    else
                        stateList.Add(new ListItem(textInfo.ToTitleCase(locationState.ToLower()), locationState));
                }

                this.photoTakenState.Items.AddRange(stateList.OrderBy(i=>i.Text).ToArray());
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                foreach (var navPoint in result.OrderBy(np => np.City))
                {
                    sb.AppendLine(string.Format("L.marker([{0}, {1}], {{ locationId: {4} }}).bindPopup(navigateToPhotoPage).bindTooltip(\"{2}, {3}\").addTo(map);", navPoint.Latitude, navPoint.Longitude, navPoint.City, navPoint.State, navPoint.LocationId));

                    var listItem = new ListItem(string.Format("{0}, {1}", navPoint.City, navPoint.State));
                    listItem.Attributes.Add("Long", navPoint.Longitude.ToString());
                    listItem.Attributes.Add("Lat", navPoint.Latitude.ToString());

                    ddlLocations.Items.Add(listItem);
                }
                
                /*
                 L.marker([51.5, -0.09]).bindPopup(navigateToPhotoPage).addTo(map);
                L.marker([51.5, -0.09]).bindPopup(navigateToPhotoPage).addTo(map);
                L.marker([51.495, -0.083]).bindPopup(navigateToPhotoPage).addTo(map);
                L.marker([51.49, -0.1]).bindPopup(navigateToPhotoPage).addTo(map);
                */

                litNavPoints.Text = sb.ToString();
            }
        }
    }
}
 