/*
 * Depends on:
 *
 *  * txtEmail
 *  * btnSendValidationEmail
 *  * Evc (hidden var)  used to validate
 */

var emailValidationModel = {
    email: null,
    validationCode: ""
}

$(function () {
    $("#EmailValidatorModal").css("z-index",11000);
    emailValidationModel.email = $("#txtEmail").val();

    $("#EmailValidatorModal").on('hidden.bs.modal',
        function () {
            // assume you cancelled out
            emailValidationModel.email = null;
        });

    $("#txtEmail").change(function () {
        var email = this.value;

        if (email && !emailValidationModel.validationCode ||
            emailValidationModel.email !== email) {
            sendValidation(email);
        }
    });

    $("#btnSendValidationEmail").click(function () {
        sendValidation();
    });
    $("#btnEmailValidation").click(function () {
        validateCode();
    });
    $("#lnkNoEmail").click(function () {
        $("#NoEmailMessage").toggleClass("hidden");
    });

    function sendValidation(email) {
        
        if (!email)
            email = $("#txtEmail").val();
        if (!email || email.indexOf("@") < 2) {
            toastr.error("Missing or invalid email address.");
            return;
        }
        $("#email-validation-code").val("");

        emailValidationModel.email = email;

        $.get("/api/account/validate/send?email=" + encodeURIComponent(email),
            function (result) {
                toastr.success("Validation code emailed to provided email address.");
            });


        $("#EmailValidatorModal").modal("show");
    }

    function validateCode() {
        var code = $("#email-validation-code").val();
        var email = $("#txtEmail").val();

        $.get("/api/account/validate/" + code +"?email=" + encodeURIComponent(email),
            function (result) {
                
                if (!result.isValidated) {
                    $("#txtEmail").removeClass("valid");
                    $("#txtEmail").addClass("invalid");
                    toastr.error("Invalid validation code. Please check your email and refresh the validation.");
                    return;
                }

                emailValidationModel.validationCode = code;

                $("#Evc").val(code);

                $("#email-validation-codeEvc").val(code);

                $("#EmailValidatorModal").modal("hide");
                $("#email-validation-code").val("");

                $("#txtEmail").removeClass("invalid");
                $("#txtEmail").addClass("valid");
                toastr.success("Email address validated.");
            });
    }
});
