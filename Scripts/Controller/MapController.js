

//angular.module('appContegralMap.map', ['ngMaterial', 'ngMessages', 'ngMap']).
//    /* Drivers controller */
//    controller('MapController', function ($scope) {
//        function OfferController(ngMap) {
//            ngMap.getMap();
//        }
//        OfferController.$inject = ["NgMap"];
//        return OfferController;
//    }).



/* Driver controller */
CustomModule.controller("MapController", function ($scope, $http, NgMap) {
    $scope.center = '6.255775, -75.574846';
    $scope.items = [];
    $scope.itemsByCity = [];
    $scope.groupItems = [];
    $scope.Ciudades = [];
    $scope.CiudadesData = [];
    $scope.Departamentos = [];
    $scope.googleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyAvEuXv35l1TrUsAtZXMO8UkEmLGAhMvL0&callback=initMap";
    $scope.progress = -1;
    $scope.showGroups = true;
    $scope.parameters = {
        Exclusive: '0',
        Department: '',
        City: '',
    };
    var $self = this;

    if ($.ServicesFramework) {
        var _sf = $.ServicesFramework(ModId);
        $self.ServiceRoot = _sf.getServiceRoot("SellingPoints");
        $self.ServicePath = $self.ServiceRoot;

        $self.Headers = {
            ModuleId: _sf.getModuleId(),
            TabId: _sf.getTabId(),
            RequestVerificationToken: _sf.getAntiForgeryValue(),
            FileFrutto: null,
        };
    }

    $scope.isShow = function (option) {
        return option == 'form'
    }

    //Next method
    $scope.next = function (form) { };

    $scope.search = () => {
        $http({
            method: "POST",
            url: `${$self.ServicePath}Points/GetPublic`,
            data: $scope.parameters,
            headers: $self.Headers,
        }).then(function (response) {
            if (response.status === 200) {
                $scope.items = response.data;
                $scope.fillMarkers($scope.items);
                $scope.groupItems = groupAndAdd($scope.items);
                $scope.showGroups = true;
            }
        });
    }


    const groupAndAdd = (arr) => {
        const res = [];
        arr.reduce((group, item) => {
            const { Ciudad } = item;
            if (!res.includes(Ciudad)) {
                res.push({ description: Ciudad });
            }
        }, {});
        return res;
    }

    $scope.showByCity = (city) => {
        $scope.itemsByCity = $scope.items.filter(c => c.Ciudad == city);
        if ($scope.itemsByCity) {
            $scope.showGroups = false;
            $scope.fillMarkers($scope.itemsByCity);
        }
    }

    $scope.fillMarkers = (items) => {
        let icon = location.host == "www.finca.co" ? 'logo-finca.png' : 'logo.png';
        items.forEach(item => {
            NgMap.getMap().then(function (map) {
                var marker = new google.maps.Marker({
                    position: {
                        lat: item.Lat,
                        lng: item.Long
                    },
                    map: map,
                    title: item.Nombre,
                    icon: window.location.origin + '/DesktopModules/SellingPoints/icons/' + icon,
                    draggable: true,
                    animation: google.maps.Animation.DROP,
                });

                $scope.makeInfoWindows(marker, item, map);
                const center = new google.maps.LatLng(item.Lat, item.Long);
                map.panTo(center);
            });
        });
    }

    $scope.makeInfoWindows = (marker, item, map) => {
        var infowindow = new google.maps.InfoWindow();
        var content =
            `<div id="content">
                <h6 class="titleInfoWindow"> ${item.Nombre}</h6> <br>
                <div id="bodyContent">
                    <p><b>${item.Ciudad} - ${item.Departamento}</b></p>
                    <p>${item.Description}</p>
                    <p>Confirma disponibilidad del producto telefónicamente <b>${item.Telefono}</b></p>
                </div>
            </div>`
        google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
            return function () {
                infowindow.setContent(content);
                infowindow.open(map, marker);
            };
        })(marker, content, infowindow));
    }

    $scope.getDepartments = () => {
        $http.get('DesktopModules/SellingPoints/Scripts/Departments.json').then(function (response) {
            $scope.Departamentos = response.data.Data;
        });

        $http.get('DesktopModules/SellingPoints/Scripts/Cities.json').then(function (response) {
            $scope.CiudadesData = response.data.Data;
        });
        $scope.Ciudades.sort();
    }

    $scope.changeCiudad = () => {
        $scope.Ciudades = $scope.CiudadesData[$scope.parameters.Department];
        $scope.Ciudades.sort();
    };


    $scope.init = () => {
        $scope.getDepartments();
        $scope.parameters.Exclusive = '0';
        $scope.search();
    }

    $scope.init();
})

    .config(function ($mdGestureProvider) {
        // For mobile devices without jQuery loaded, do not
        // intercept click events during the capture phase.
        $mdGestureProvider.skipClickHijack();
    });