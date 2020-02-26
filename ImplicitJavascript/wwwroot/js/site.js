const XenaApi = "https://my.xena.biz/Api";
const XenaLogin = "https://login.xena.biz";
const ClientId = "[YOUR CLIENT ID]";

var config = {
    authority: XenaLogin,
    client_id: ClientId,
    redirect_uri: window.location.origin + "/callback.html",
    post_logout_redirect_uri: window.location.origin + "/index.html",

    // these two will be done dynamically from the buttons clicked, but are
    // needed if you want to use the silent_renew
    response_type: "id_token token",
    scope: "openid profile testapi",
    // this will toggle if profile endpoint is used
    loadUserInfo: true,

    // silent renew will get a new access_token via an iframe 
    // just prior to the old access_token expiring (60 seconds prior)
    silent_redirect_uri: window.location.origin + "/silent.html",
    automaticSilentRenew: true,
};
Oidc.Log.logger = window.console;
Oidc.Log.level = Oidc.Log.DEBUG;

var mgr = new Oidc.UserManager(config);

mgr.events.addUserLoaded(function (user) {
    log("User loaded");
    showTokens();
    setUserLogin();
});
mgr.events.addUserUnloaded(function () {
    log("User logged out locally");
    showTokens();
});
mgr.events.addAccessTokenExpiring(function () {
    log("Access token expiring...");
});
mgr.events.addSilentRenewError(function (err) {
    log("Silent renew error: " + err.message);
});
mgr.events.addUserSignedOut(function () {
    log("User signed out of OP");
});

function popup() {
    mgr.signinPopup().then(function () {
        log("Logged In");
    });
}

function loginLogout() {
    mgr.getUser().then(function (user) {
        if (user == null) {
            login();
        } else {
            logout();
        }
    })
}

function login() {
    log("user login");
    mgr.signinRedirect();
}

function logout() {
    log("user logout");
    mgr.signoutRedirect();
}

function revoke() {
    mgr.revokeAccessToken().then(function () {
        log("Access Token Revoked.")
    }).catch(function (err) {
        log(err);
    });
}

function renewToken() {
    mgr.signinSilent()
        .then(function () {
            log("silent renew success");
            showTokens();
        }).catch(function (err) {
            log("silent renew error", err);
        });
}

function callApi(access_token, url) {
    var promiseObj = new Promise(function (resolve, reject) {
        $.ajax({
            url: url,
            type: "GET",
            beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', "Bearer " + access_token); },
            success: function (response) {
                resolve(response);
            }
        });
    });
    return promiseObj;
}

function callUserMembership() {
    mgr.getUser().then(function (user) {
        callApi(user.access_token, XenaApi + "/User/XenaUserMembership?forceNoPaging=true").then(function (response) {
            $('#ajax-result')[0].innerHTML = JSON.stringify(response, null, 2);
            log("user membership");
        })
    });
}

function callUserFiscalSetup() {
    mgr.getUser().then(function (user) {
        callApi(user.access_token, XenaApi + "/User/FiscalSetup?forceNoPaging=true").then(function (response) {
            $('#ajax-result')[0].innerHTML = JSON.stringify(response, null, 2);
            log("user fiscal");
        })
    });
}

function callUserApps() {
    $('#ajax-result')[0].innerHTML = "";
    mgr.getUser().then(function (user) {
        callApi(user.access_token, XenaApi + "/User/XenaUserMembership?ForceNoPaging=true").then(function (response) {
            var entities = response.Entities;
            for (var i = 0; i < entities.length; i++) {
                callApi(user.access_token, XenaApi + "/Fiscal/" + entities[i].FiscalSetupId + "/XenaApp").then(function (response) {
                    $('#ajax-result')[0].innerHTML += JSON.stringify(response, null, 2) + '\r\n';
                    log("user apps");
                });
            }
        })
    });
}

function setUserLogin() {
    $("#loginLogout")[0].innerHTML = 'Login';
    mgr.getUser()
        .then(function (user) {
            if (user == null) {
                return;
            }
            var name = user.profile.preferred_username;
            $("#loginLogout")[0].innerHTML = 'Logout: ' + name;
        });
}
setUserLogin();

if (window.location.hash) {
    window.location.hash = decodeURIComponent(window.location.hash).replace('#', '?');
    handleCallback();
}

document.querySelector("#loginLogout").addEventListener("click", loginLogout, false);
document.querySelector("#renew").addEventListener("click", renewToken, false);
document.querySelector("#userMembership").addEventListener("click", callUserMembership, false);
document.querySelector("#userFiscalSetup").addEventListener("click", callUserFiscalSetup, false);
document.querySelector("#userApps").addEventListener("click", callUserApps, false);

function log(data) {
    document.getElementById('response').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('response').innerHTML += msg + '\r\n';
    });
}

function display(selector, data) {
    if (data && typeof data === 'string') {
        try {
            data = JSON.parse(data);
        }
        catch (e) { }
    }
    if (data && typeof data !== 'string') {
        data = JSON.stringify(data, null, 2);
    }
    document.querySelector(selector).textContent = data;
}

function showTokens() {
    mgr.getUser().then(function (user) {
        if (user) {
            display("#id-token", user);
        }
        else {
            log("Not logged in");
        }
    });
}
showTokens();

function handleCallback() {
    mgr.signinRedirectCallback().then(function (user) {
        var hash = window.location.hash.substr(1);
        var result = hash.split('&').reduce(function (result, item) {
            var parts = item.split('=');
            result[parts[0]] = parts[1];
            return result;
        }, {});

        log(result);
        showTokens();

        window.history.replaceState({},
            window.document.title,
            window.location.origin + window.location.pathname);

    }, function (error) {
        log(error);
    });
}