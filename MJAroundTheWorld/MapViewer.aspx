<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MapViewer.aspx.cs" Inherits="MJAroundTheWorld.MapViewer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MJ Around the World - Map Viewer</title>
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.0.3/dist/leaflet.css" integrity="sha512-07I2e+7D8p6he1SIM+1twR5TIrhUQn9+I6yjqD53JQjFiMf8EtC93ty0/5vJTZGF8aAocvHYNEDJajGdNx1IsQ==" crossorigin=""/>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous">
    <script src="https://unpkg.com/leaflet@1.0.3/dist/leaflet.js" integrity="sha512-A7vV8IFfih/D732iSSKi20u/ooOfj/AGehOKq0f4vLT1Zr2Y+RX7C+w8A1gaSasGtRUZpF/NZgzSAu4/Gc41Lg==" crossorigin=""></script>
    <script src="https://code.jquery.com/jquery-3.4.1.min.js" integrity="sha256-CSXorXvZcTkaix6Yvo6HppcZGetbYMGWSFlBw8HfCJo=" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>
    <style>
		#map {
			width: 100%;
			height: 520px;
		}

        .whereWeveBeen
        {
			z-index: 999999;
			font-size: small;
            font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif;
        }

        .required {
            color: #f00;
            font-weight: bolder;
        }

        .section-header {
            border: 1px solid #1d65a6;
            background: #1d65a6;
            padding: 4px 0px 4px 8px;
            height: 32px;
        }

        .section-header h3 {
            color: #fff;
            font-size: 16.56px;
            font-family: "Segoe UI Semilight", "Segoe UI", "Segoe", Tahoma, Arial, Helvetica, sans-serif;
            font-weight: 300;
        }
	</style>
</head>
<body>
    <form method="post" enctype="multipart/form-data" id="uploadForm" runat="server">
        <div id="main" class="container-fluid">
            <div class="row">
                <div id="photo-section" class="col-xs-12 col-md-4 embed-responsive">
                    <iframe name="PhotoFrame" src="Photo/Index"></iframe>
                </div>
                <div id="map-section" class="col-xs-12 col-md-4">
                    <div class="section-header">
                        <h3>Where We've Been</h3>
                    </div>
                    <div id="map">
                    </div>
                    <div id="zoom" class="container">
                        <div class="row form-group">
                            <label for="zoom-picker" class="col-8 text-right col-form-label">Zoom to location:</label>
                            <asp:DropDownList ID="ddlLocations" CssClass="form-control col-4" runat="server" AutoPostBack="false" onchange="changeWhereWeveBeen(this)">
                                <asp:ListItem Value="Select a location" Text="Select a Location"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div id="upload-section" class="col-xs-12 col-md-4">
                    <div class="section-header">
                        <h3>Upload Photo</h3>
                    </div>                
                    <div class="form-group">
                        <label for="upload-button">Image to Upload<span class="required">*</span></label>
                        <input type="file" class="form-control-file" name="file" id="upload-button" accept="image/*"/>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <label for="photoTakenCity">City<span class="required">*</span></label>
                            <input type="text" class="form-control" id="photoTakenCity" name="photoTakenCity" placeholder="Enter a city" />
                        </div>
                        <div class="form-group col-md-6">
                            <label for="photoTakenState">State<span class="required">*</span></label>
                            <select id="photoTakenState" class="form-control" name="photoTakenState" runat="server">
                                <option selected>Choose...</option>
                            </select>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="photoTakenDate">Date Taken<span class="required">*</span></label>
                        <input type="date" class="form-control" id="photoTakenDate" name="photoTakenDate" />
                    </div>
                    <div class="form-group">
                        <label for="photoSubmittedBy">Submitted by<span class="required">*</span></label>
                        <input type="text" id="photoSubmittedBy" name="photoSubmittedBy" class="form-control" placeholder="Employee name" />
                    </div>
                    <div class="form-group">
                        <label for="description">Brief description</label>
                        <textarea class="form-control" id="description" name="description" rows="3"></textarea>
                    </div>
                    <button type="submit" class="btn btn-success mt-2" id="submitButton">Submit</button>
                    <div class="alert alert-success fade" role="alert" id="successAlert">
                        <strong>Success!</strong> Your photo has been submitted.
                    </div>
                    <div class="alert alert-danger fade mt-2" role="alert" id="failureAlert">
                        Your photo failed to upload. Please try again or contact the help desk for assistance.
                        <div id="error-msg"></div>
                    </div>
                </div>
            </div>
        </div>
    </form>
        <!--<div class="whereWeveBeen">
        </div>
        <br />
        <table width="100%">
            <tr>
                <td style="width:50%;vertical-align:top;">
                    <div id="map"></div>
                </td>
                <td style="width:50%;vertical-align:top;">
                    <iframe style="margin-top: -9px" width="100%" height="619px" frameborder="0" name="PhotoFrame" src="Photo/Index"></iframe>
                </td>
            </tr>
        </table>-->
    <script type="text/javascript">
        var map;
        function changeWhereWeveBeen(ddlWhereWeveBeen) {
            var selectedOption = ddlWhereWeveBeen.options[ddlWhereWeveBeen.selectedIndex];

            if (selectedOption.getAttribute('Long') !== null) {
                map.setView([parseFloat(selectedOption.getAttribute('Lat')), parseFloat(selectedOption.getAttribute('Long'))], 8);
            }
        }
        function navigateToPhotoPage(markerObj) {
            //window.open("http://localhost:48802/Photo/Index/" + markerObj.options.locationId.toString(), "PhotoFrame");
            window.open("http://gr-web-02/MJAroundTheWorld/Photo/Index/" + markerObj.options.locationId.toString(), "PhotoFrame");
        }

        map = L.map('map').setView([44.765, -99.667], 3);

        L.tileLayer('https://api.tiles.mapbox.com/v4/{id}/{z}/{x}/{y}.png?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
            id: 'mapbox.satellite'
        }).addTo(map);

        <asp:Literal id="litNavPoints" runat="server" />
        $(document).ready(function () {
            $('#uploadForm').on('submit', function (e) {
                e.preventDefault();
                var form = $('#uploadForm')[0];

                $.ajax({
                    type: "POST",
                    url: "Photo/Upload",
                    processData: false,
                    contentType: false,
                    data: new FormData(form)
                }).fail(function (r, s, e) {
                    debugger;
                    $("#error-msg").text(r.responseText.match(/\[HttpException\]: (.*)/)[1]);
                    $('#failureAlert').toggleClass('show');
                    setTimeout(function () { $('#failureAlert').toggleClass('show'); }, 5000);
                }).done(function () {
                    $('#successAlert').toggleClass('show');
                    setTimeout(function () { $('#successAlert').toggleClass('show'); }, 3000);
                    form.reset();
                 });

                return false;
            });
        });
    </script>
</body>
</html>

