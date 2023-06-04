/* Driver controller */
CustomModule.controller("AdminController", function ($scope, $http, NgMap) {
    const INDICE_INICIAL = 0;
    const PAGE_INICIAL = 0;
    $scope.Items = {
        HasNextPage: false,
        HasPreviousPage: false,
        IsFirstPage: false,
        IsLastPage: false,
        PageCount: 0,
        PageIndex: 0,
        PageSize: 10,
        TotalCount: 0
    };
    $scope.gap = 10;
    $scope.index = 0;
    $scope.center = '6.255775, -75.574846';
    $scope.googleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyCjiqW2J0yWumnHzg1VxOI5-uMeftpwGr0&callback=initMap";
    $scope.myLatLng = {
        lat: 6.255775,
        lng: -75.574846
    };
    $scope.disableBtn = false;
    $scope.first = true;
    $scope.puntoventa = {
        Exclusive: '0',
    };
    $scope.Ciudades = [];
    $scope.CiudadesData = [];
    $scope.Departamentos = [];
    $scope.progress = -1;
    $scope.showModal = false;
    $scope.showFillCSV = false;

    var $self = this;
    if ($.ServicesFramework) {
        var _sf = $.ServicesFramework(ModuleId);
        $self.ServiceRoot = _sf.getServiceRoot("SellingPoints");
        $self.ServicePath = $self.ServiceRoot;
        $self.Headers = {
            ModuleId: ModuleId,
            TabId: _sf.getTabId(),
            RequestVerificationToken: _sf.getAntiForgeryValue(),
            FileFrutto: null,
        };
    }


    NgMap.getMap().then(function (map) {
        let icon = '/DesktopModules/SellingPoints/icons/';
        icon += location.host === "www.finca.co" ? 'logo-finca.png' : 'logo.png';
        var markerD = new google.maps.Marker({
            position: $scope.myLatLng,
            map: map,
            title: 'Contegral',
            icon: window.location.origin + icon,
            draggable: true,
            animation: google.maps.Animation.DROP,
            //label: citymap[city].population.ToString()
        });

        google.maps.event.addListener(map, 'click', function (event) {
            map.markers.marker = null;
            var lat = event.latLng.lat();
            var lng = event.latLng.lng();
            $scope.puntoventa.lat = lat;
            document.getElementById('lat').value = lat;
            document.getElementById('long').value = lng;
            $scope.puntoventa.long = lng;
            markerD = null;
            markerD = new google.maps.Marker({
                position: event.latLng,
                map: map,
                title: $scope.puntoventa.Nombre,
                icon: window.location.origin + icon,
                draggable: true,
                animation: google.maps.Animation.DROP,
            });
        });

    }).catch(e => console.log(e));

    $scope.initializeFilter = (searchText = '') => {
        $scope.filters = {
            PageNumber: INDICE_INICIAL,
            PageSize: PAGE_INICIAL,
            Search: searchText
        };
    }

    $scope.search = () => {
        $scope.initializeFilter($scope.filters.Search);
        $scope.getItems();
    }

    $scope.getItems = () => {
        $http({
            method: "POST",
            url: `${$self.ServicePath}Points/GetPointsAdmin`,
            data: $scope.filters,
            headers: $self.Headers,
        }).then(function (response) {
            if (response.status === 200) {
                $scope.Items = response.data;
            }
        }).catch(function (err) {
            console.log(err);
        });
    }


    $scope.publicItem = function (itemId) {
        $http({
            method: "PUT",
            url: `${$self.ServicePath}Points/Public?itemId=${itemId}`,
            headers: $self.Headers,
        }).then(function (response) {
            if (response.status === 200) {
                $scope.getItems();

            }
        });
    };

    $scope.prevPage = function () {
        if ($scope.Items.PageIndex > 0) {
            $scope.Items.PageIndex--;
            $scope.filters.PageNumber = $scope.Items.PageIndex;
            $scope.getItems();
        }
    }

    $scope.nextPage = function () {
        if ($scope.Items.PageIndex < $scope.Items.PageCount) {
            $scope.Items.PageIndex++
            $scope.filters.PageNumber = $scope.Items.PageIndex;
            $scope.getItems();
        }
    }

    $scope.setPage = function (page) {
        $scope.filters.PageNumber = page;
        $scope.getItems();
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

    $scope.load = function () {
        getItems(0, true);
    }

    $scope.editform = (id) => {

        $http({
            method: "Get",
            url: `${$self.ServicePath}Points/GetById?id=${id}`,
            headers: $self.Headers,
        }).then(function (response) {
            if (response.status === 200) {
                $scope.showForm = true;
                $scope.puntoventa = response.data;
                var map = NgMap.getMap().then(function (map) {
                    var marker = new google.maps.Marker({
                        position: {
                            lat: $scope.puntoventa.Lat,
                            lng: $scope.puntoventa.Long
                        },
                        map: map,
                        title: $scope.puntoventa.Nombre,
                        icon: window.location.origin + '/DesktopModules/SellingPoints/icons/logo.png',
                        draggable: true,
                        animation: google.maps.Animation.DROP,
                    });
                });
                $scope.changeCiudad();
                $scope.puntoventa.Ciudad = response.data.Ciudad;
                return;
            }

        },
            function (response) {
                if (response.status === 500) {
                    showAlert('Ha ocurrido un error al tratar de guardar la información')
                }
            });
    };

    $scope.save = () => {
        if (!pageIsValid('WebForm')) {
            return;
        }

        $http({
            method: "POST",
            url: `${$self.ServicePath}Points/Post`,
            headers: $self.Headers,
            data: $scope.puntoventa,
        }).then(function (response) {
            if (response.status === 200) {
                $scope.puntoventa = {};
                $scope.file = null;
                $scope.disableBtn = false;
                $scope.progress = -1;
                $scope.showModal = true;
                msjWithHtml('Operación completada con éxito!', `La información se ha guardado correctamente!`, 'success');
                $scope.getItems();
                return;
            }

        },
            function (response) {
                if (response.status === 500) {
                    msjWithHtml('La operación no pudo ser procesada!', `Ha ocurrido un error al tratar de guardar la información`, 'error');
                }
            });
    }

    $scope.getFile = function (files) {
        $scope.file = files;
        var ul = document.createElement('ul');
        var conten = document.getElementById('contenfiles');
        angular.forEach($scope.file, function (value, key) {
            var li = document.createElement('li');
            li.innerText = value.name;
            li.appendChild = document.createTextNode(value.name);
            ul.appendChild(li);
        });
        conten.insertAdjacentElement("beforebegin", ul);
    }

    $scope.showUpload = () => {
        $scope.showFillCSV = true;
    }

    $scope.uploadFile = function () {
        if (!$scope.file) {
            msjWithHtml('Oops!', "Por favor adjunte un archivo con extensión .CSV para poder realizar el envio de la información", 'warning');
            return
        }
        if (!$scope.file[0].type.includes('csv')) {
            msjWithHtml('Oops!', "Por favor Adjunte un archivo con extension .csv", 'warning');
            return
        }
        showLoading();
        if ($scope.file != null && $scope.file != '') {
            var fd = new FormData();
            //Take the first selected file
            for (var i = 0; i < $scope.file.length; i++) {
                fd.append("file", $scope.file[i]);
            }
            $http.post($self.ServicePath + 'Points/AddRecords', fd, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.file = [];
                $scope.picture = null;
                $scope.filename = "";
                document.getElementById('Adjunto').value = null;
                msjWithHtml('Operación completada con éxito!', `La información del archivo se ha cargado correctamente! <br> ${response.data}`, 'success');
                $scope.getItems();
                hideLoading();
            },
                function (response) {
                    //ShowToast('Error al cargar los departamentos, por favor inténtalo de nuevo.', 'error');
                    hideLoading();
                }
            );
        }
        else {
            $scope.file = [];
            document.getElementById('Adjunto').value = '';
            $scope.send();
            hideLoading();
        }
    };

    getDepartments = () => {
        $http.get('DesktopModules/SellingPoints/Scripts/Departments.json').then(function (response) {
            $scope.Departamentos = response.data.Data;
        });
        $http.get('DesktopModules/SellingPoints/Scripts/Cities.json').then(function (response) {
            $scope.CiudadesData = response.data.Data;
        });

        $scope.Ciudades.sort();
    }

    $scope.changeCiudad = function () {

        $scope.Ciudades = $scope.CiudadesData[$scope.puntoventa.Departamento];
        $scope.Ciudades.sort();

    };

    getDepartments();
    $scope.initializeFilter();
    $scope.getItems();
});
