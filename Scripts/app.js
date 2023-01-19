var CustomModule;
(function () {

    const settings = { isAngular: true, requiredMode: 'span' }

    if (settings.isAngular) {
        var ngAppExist = $('html').find('[ng-app=appContegralMap]');
        if (ngAppExist.length == 0) {
            $('body').attr('ng-app', 'appContegralMap');
            CustomModule = angular.module('appContegralMap', [
                'ngMaterial',
                'ngMessages',
                'ngMap'
            ]);
        }
    }

    //Valida el estado de la página, que cumpla con: requeridos, campo email, campo solo número, entre otros.
    pageIsValid = function () {
        let inputValid = validateInput();
        let selectValid = validateSelect();


        return inputValid && selectValid;
    };

    //Manejador de los campos input
    validateInput = function () {
        let isValid = true;
        var required = document.querySelectorAll('input[ax-required=true]');
        if (required.length > 0) {
            for (var i = 0; i < required.length; i++) {
                if (!required[i].value) {
                    switch (settings.requiredMode) {
                        case 'span':
                            createRequired(required, i);
                            break;
                        case 'modal':
                            break;
                    }

                    isValid = false;
                }
            }
        }
        return isValid;
    }

    //Crea el texto "Campo requerido" en el documento.
    createRequired = function (required, i) {
        let messageExist = required[i].parentElement.querySelector('.error-required');
        if (!messageExist) {
            let requiredMessage = required[i].getAttribute('ax-required-text');
            let span = document.createElement('span');
            span.innerText = requiredMessage;
            span.setAttribute('class', 'error-required');
            required[i].parentElement.appendChild(span)

            switch (required[i].type) {
                case 'text':
                    required[i].addEventListener('keyup', function (e) {
                        if (e.currentTarget.value) {
                            let aux = required[i].parentElement.querySelector('.error-required');
                            if (aux) {
                                aux.parentNode.removeChild(aux);
                            }
                        }
                    });
                    break;
                case 'select-one':
                    required[i].addEventListener('change', function (e) {
                        if (e.currentTarget.value) {
                            let aux = required[i].parentElement.querySelector('.error-required');
                            if (aux) {
                                aux.parentNode.removeChild(aux);
                            }
                        }
                    });
                    break;

            }
        }
    }

    validateSelect = function () {
        let isValid = true;
        var required = document.querySelectorAll('select[ax-required=true]');
        if (required.length > 0) {
            for (var i = 0; i < required.length; i++) {
                if (!required[i].value || required[i].value == '0' || required[i].value == '' || required[i].value == '? undefined:undefined ?') {
                    switch (settings.requiredMode) {
                        case 'span':
                            createRequired(required, i);
                            break;
                        case 'modal':
                            break;
                    }

                    isValid = false;
                }
            }
        }
        return isValid;
    }

    showAlert = function (message, type) {
        let alertExist = document.querySelector('.custom-alert');
        if (alertExist) {
            alertExist.parentNode.removeChild(alertExist);
        }

        let body = document.querySelector('body');
        let html = document.createElement('div');
        html.setAttribute('class', 'custom-alert ' + type);
        html.innerHTML = message
        body.appendChild(html);

        setTimeout(function () {
            let alertExist = document.querySelector('.custom-alert');
            if (alertExist) {
                alertExist.parentNode.removeChild(alertExist);
            }
        }, 30000);
    }


    showLoading = function () {
        let aux = document.querySelector('#panelCargando');
        if (!aux) {
            let html = document.createElement('div');
            html.id = 'panelCargando';
            let span = document.createElement('span');
            span.innerHTML = 'Cargando...';
            let backgorund = document.createElement('div');
            backgorund.setAttribute('class', 'background');
            html.appendChild(backgorund);
            html.appendChild(span);
            document.querySelector('body').appendChild(html);
        }
    };

    hideLoading = function () {
        let aux = document.querySelector('#panelCargando');
        if (aux) {
            aux.parentNode.removeChild(aux);
        }
    };

    isNumberKey = (e) => {
        tecla = (document.all) ? e.keyCode : e.which;

        //Tecla de retroceso para borrar, siempre la permite
        if (tecla == 8) {
            return true;
        }

        // Patron de entrada, en este caso solo acepta numeros
        patron = /[0-9]/;
        tecla_final = String.fromCharCode(tecla);
        return patron.test(tecla_final);
    }


})();