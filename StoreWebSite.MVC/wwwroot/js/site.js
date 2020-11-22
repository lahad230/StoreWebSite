//$("#submit").click(function () {
//    var data = $('#form').serialize()
//    console.log(data);
//    $.ajax({
//        type: 'POST',
//        url: '/User/Login',
//        data: data,
//        success: function (data) {
//        },
//        error: function (err) {
//        }
//    });
//});
//$(function () {
//    $('#submit').on('click', function (evt) {
//        evt.preventDefault();
//        $.post('/user/login', $("#form").serialize(), function () {
//            alert('Posted using jQuery');
//        });
//    });
//});