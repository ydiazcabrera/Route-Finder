 //ToDo - Change this to come in from ViewModel

function initMap() {
    //Create a new map
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 12,
        center: mapCenter,
    mapTypeId: 'terrain'
});



//Create a line from the routeCoordinates
var routePath = new google.maps.Polyline({
    path: routeCoordinates,
    geodesic: true,
    strokeColor: '#0000FF',
    strokeOpacity: 1,
    strokeWeight: 3
});

//Create a line from the fastestRouteCoordinates
var fastRoutePath = new google.maps.Polyline({
    path: fastestCoordinates,
    geodesic: true,
    strokeColor: '#FF0000',
    strokeOpacity: 1,
    strokeWeight: 3
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
        //var sensorBuffer = new google.maps.Circle({
        //    strokeColor: '#FF0000',
        //    strokeOpacity: 0.8,
        //    strokeWeight: 1,
        //    fillColor: '#FF0000',
        //    fillOpacity: 0.35,
        //    map: map,
        //    center: myLatlng,
        //    radius: 300,
        //    label: data.name,
        //});
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
    else if (data.aqi >= 30 && data.aqi <= 50) {
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
    else if (data.aqi <= 30) {
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
}

    routePath.setMap(map);
    fastRoutePath.setMap(map);

        }