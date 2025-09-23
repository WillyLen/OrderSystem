
$(function(){
    
    // $("#f-company-ci").change(function(){
    //     if( $("#f-company-ci").is(':checked') ){
    //         $("#login-area").addClass("d-none");
    //         $("#btn-login").addClass("d-none");
    //         $("#btn-enter").removeClass("d-none");

    //     }else{
    //         $("#login-area").removeClass("d-none");
    //         $("#btn-login-style001").removeClass("d-none");
    //         $("#btn-enter").addClass("d-none");
    //     }
    // })

    // $("#f-company-other").change(function(){
    //     if( $("#f-company-other").is(':checked') ){
    //         $("#login-area").removeClass("d-none");
    //         $("#btn-login").removeClass("d-none");
    //         $("#btn-enter").addClass("d-none");
    //     }else{
    //         $("#login-area").addClass("d-none");
    //         $("#btn-login").addClass("d-none");
    //         $("#btn-enter").removeClass("d-none");
    //     }
    // })


    //Show password
    //--------------------------------------------------
    //with bootstrap 4 css "d-none"
    $("#f-btn-showpw").click(function(){
        $("#f-btn-hidepw").removeClass("d-none");
        $("#f-password").attr("type", "password");
        $(this).addClass("d-none");
    });

    $("#f-btn-hidepw").click(function(){
        $("#f-btn-showpw").removeClass("d-none");
        $("#f-password").attr("type", "text");
        $(this).addClass("d-none");
    });
    //--------------------------------------------------


});