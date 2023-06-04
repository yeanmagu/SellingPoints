CustomModule.controller("MapController", function ($scope, $http, NgMap) {
    $scope.center = '6.255775, -75.574846';
    const INDICE_INICIAL = 0;
    const PAGE_INICIAL = 0;
    $scope.Items = {
        HasNextPage: false,
        HasPreviousPage: false,
        IsFirstPage: false,
        IsLastPage: false,
        PageCount: 0,
        PageIndex: 0,
        PageSize: 100,
        TotalCount: 0
    };
    $scope.itemsByCity = [];
    $scope.groupItems = [];
    $scope.Ciudades = [];
    $scope.CiudadesData = [];
    $scope.Departamentos = [];
    $scope.googleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyCjiqW2J0yWumnHzg1VxOI5-uMeftpwGr0&callback=initMap";
    $scope.progress = -1;
    $scope.showGroups = true;
    $scope.parameters = {
        PageNumber: INDICE_INICIAL,
        PageSize: PAGE_INICIAL,
        Exclusive: '0',
        Department: '',
        City: '',
    };
    $scope.parametersByCity = {
        PageNumber: INDICE_INICIAL,
        PageSize: PAGE_INICIAL,
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

    $scope.prevPage = function () {
        if ($scope.items.PageIndex > 0) {
            $scope.items.PageIndex--;
            $scope.parameters.PageNumber = $scope.items.PageIndex;
            $scope.search();
        }
    }

    $scope.nextPage = function () {
        if ($scope.items.PageIndex < $scope.items.PageCount) {
            $scope.items.PageIndex++
            $scope.parameters.PageNumber = $scope.items.PageIndex;
            $scope.search();
        }
    }

    $scope.setPage = function (page) {
        $scope.parameters.PageNumber = page;
        $scope.search();
    };

    $scope.range = function (size, start, end) {
        var ret = [];
        if (size < end) {
            end = size;
            start = size - $scope.gap;
        }
        for (var i = start; i < end; i++) {
            ret.push(i);
        }
        return ret;
    };

    //Next method
    $scope.next = function (form) { };
    $scope.searchFilter = () => {
        $scope.parameters.PageNumber = INDICE_INICIAL;
        $scope.parameters.PageNumber = PAGE_INICIAL;
        $scope.search();
    }
    $scope.search = () => {
        $http({
            method: "POST",
            url: `${$self.ServicePath}Points/GetPublic`,
            data: $scope.parameters,
            headers: $self.Headers,
        }).then(function (response) {
            if (response.status === 200) {
                $scope.items = response.data;
                //$scope.fillMarkers($scope.items);
                $scope.groupItems = $scope.items.data//groupAndAdd($scope.items);
                $scope.showGroups = true;
            }
        });
    }

    $scope.showByCity = (city, department) => {
        $scope.parametersByCity.City = city;
        $scope.parametersByCity.Department = department;
        $http({
            method: "POST",
            url: `${$self.ServicePath}Points/GetPublicByCity`,
            data: $scope.parametersByCity,
            headers: $self.Headers,
        }).then(function (response) {
            if (response.status === 200) {
                $scope.Items = response.data;
                $scope.itemsByCity = $scope.Items.Data;
                $scope.showGroups = false;
                $scope.fillMarkers($scope.itemsByCity);
            }
        });
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