 //ToDo - Change this to come in from ViewModel

function initMap() {
    //Create a new map
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 13,
        center: mapCenter,
    mapTypeId: 'terrain'
    });

    //Create a line from the fastestRouteCoordinates
    var fastRoutePath = new google.maps.Polyline({
        path: fastestCoordinates,
        geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 1,
        strokeWeight: 3
    });
    //Create a line from the routeCoordinates
    var routePath = new google.maps.Polyline({
        path: routeCoordinates,
        geodesic: true,
        strokeColor: '#0000FF',
        strokeOpacity: 1,
        strokeWeight: 3
    });

  var marker = new google.maps.Marker({
    position: routeCoordinates[0],
    map: map,
    label: 'Start'
});

var marker = new google.maps.Marker({
    position: routeCoordinates[routeCoordinates.length - 1],
    map: map,
    label: 'End'
});


    //Displays the sensors that were passed in as "ViewBag.Markers"
    for (i = 0; i < sensors.length; i++) {
        var data = sensors[i]
        var myLatlng = new google.maps.LatLng(data.lat, data.lng);
        var north = sensors[i].north;
        var south = sensors[i].south;
        var east = sensors[i].east;
        var west = sensors[i].west;
        var marker = new google.maps.Marker({
            //opacity: 0,
            labelAnchor: { x: 0, y: 30 },
            position: myLatlng,
            map: map,
            title: data.title,
            scaledSize: new google.maps.Size(50, 50),
            icon: data.aqi/*'http://maps.google.com/mapfiles/kml/pal4/icon57.png'*/,
            label: data.aqi,
        });

        //var sensorBuffer = new google.maps.Rectangle({
        //    strokeColor: '#FF0000',
        //    strokeOpacity: 0.8,
        //    strokeWeight: 1,
        //    fillColor: '#FF0000',
        //    fillOpacity: 0.35,
        //    map: map,
        //    bounds: {
        //        north: ,
        //        south: ,
        //        east: ,
        //        west:
        //    }
        //    label: data.name,
        //});


        if (data.aqi >= 151 && data.aqi <= 200) {
            var sensorBuffer = new google.maps.Circle({
                strokeColor: '#FF0000',
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: '#FF0000',
                fillOpacity: 0.35,
                map: map,
                center: myLatlng,
                radius: 300,
                label: data.name,
            });
            var sensorBuffer = new google.maps.Rectangle({
                strokeColor: '#FF0000',
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: '#FF0000',
                fillOpacity: 0.35,
                map: map,
                bounds: {
                    north: north,
                    south: south,
                    east: east,
                    west: west
                }
                //label: data.name
            });
        }
        else if (data.aqi >= 101 && data.aqi <= 150) {
            var sensorBuffer = new google.maps.Circle({
                strokeColor: '#E17E00',
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: '#E17E00',
                fillOpacity: 0.35,
                map: map,
                center: myLatlng,
                radius: 300,
                label: data.name,
            });
        }
        //else if (data.aqi >= 51 && data.aqi <= 100) {
        //    var sensorBuffer = new google.maps.Circle({
        //        strokeColor: '#E1E100',
        //        strokeOpacity: 0.8,
        //        strokeWeight: 1,
        //        fillColor: '#E1E100',
        //        fillOpacity: 0.35,
        //        map: map,
        //        center: myLatlng,
        //        radius: 300,
        //        label: data.name,
        //    });
        //}
        else if (data.aqi >= 51 && data.aqi <= 100) {
            var sensorBuffer = new google.maps.Circle({
                strokeColor: '#E1E100',
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: '#E1E100',
                fillOpacity: 0.35,
                map: map,
                center: myLatlng,
                radius: 300,
                label: data.name,
            });
        }
        else if (data.aqi <= 50) {
            var sensorBuffer = new google.maps.Circle({
                strokeColor: '#00E400',
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: '#00E400',
                fillOpacity: 0.35,
                map: map,
                center: myLatlng,
                radius: 300,
                label: data.name,
            });
        }
        else if (data.aqi === "no data") {
            var sensorBuffer = new google.maps.Circle({
                strokeColor: '#b3b3b3',
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: '#b3b3b3',
                fillOpacity: 0.35,
                map: map,
                center: myLatlng,
                center: myLatlng,
                radius: 300,
                label: data.name,
            });
        }

    }

    var iconBase = 'https://maps.google.com/mapfiles/kml/shapes/';
    var icons = {
        parking: {
            name: 'Parking',
            icon: iconBase + 'parking_lot_maps.png'
        },
        library: {
            name: 'Library',
            icon: iconBase + 'library_maps.png'
        },
        info: {
            name: 'Info',
            icon: iconBase + 'info-i_maps.png'
        }
    };

    var legend = document.getElementById('legend');

    var div = document.createElement('div');
        div.innerHTML = 
            `<div style="pointer-events: none;" id="legend-div>
                <div class="row">
                    <div class="col-md-12">
                        <div class="my-legend">
                            <div class="legend-title"><h4>AQI</h4></div>
                            <div class="legend-scale">
                                <ul class="legend-labels">
                                    <li><span style="background:#00E400;"></span>Good 0 - 50</li>
                                    <li><span style="background:#E1E100;"></span>Moderate 51 - 100</li>
                                    <li><span style="background:#E17E00;"></span>Unhealthy for Sensitive Groups 101 - 150</li>
                                    <li><span style="background:#FF0000;"></span>Unhealthy 151 - 200</li>
                                    <li><span style="background:#8F3F97;"></span>Hazardous 201 +</li>
                            <div class="legend-title"><h4>Paths</h4></div>
                                    <li style="display: flex; vertical-align: sub"><span id="safeRoute"></span>Healthy Path</li>
                                    <li><span id="fastRoute"></span>Fast Path</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`;

        legend.appendChild(div);
    setTimeout(function () {
        document.getElementById("legend").style.display = "block";
    }, 1000)
    

    map.controls[google.maps.ControlPosition.LEFT_CENTER].push(legend);

    fastRoutePath.setMap(map);
    routePath.setMap(map);
}