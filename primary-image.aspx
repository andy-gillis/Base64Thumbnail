<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <style>
        body {font: normal .84em 'segoe ui'; padding: .5em; padding-bottom: 2em}
        input, textarea {font: normal 1em 'segoe ui'}
    </style>
    <script src="http://code.jquery.com/jquery-1.12.4.min.js"></script>
    <script>
        function parseWebsite() {
            var url = "/primaryimage?url=" + encodeURIComponent($("#url").val());
            var transGif = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
            $("#serviceurl").text("http://services.ayespee.net" + url);
            $.ajax({
                url: url,
                async: false
            })
            .done(function (data) {
                $("#ta").val(data);
                $("#image1").attr("src", $("#ta").val());
            })
            .fail(function (xhr) {
                $("#ta").val("Request Status: " + xhr.status + " Status Text: " + xhr.statusText + " " + xhr.responseText);
                $("#image1").attr("src", transGif);
            });
        }
    </script>
</head>
<body>
    <div>Enter webpage url to locate primary image...</div>
    <div>
        <input type="text" name="url" id="url" value="https://www.forbes.com/sites/markjoyella/2017/08/29/rachel-maddow-ends-august-as-number-one-in-cable-news/#605f66b56b63" style="width:65%" />
        <input type="button" name="submit" id="submit" value="Submit" onclick="parseWebsite()" style="height:1.8em" />
    </div>
    <div>&#160;<br />Calling: <span name="serviceurl" id="serviceurl">[service]</span></div>
    <div>&#160;<br />Primary image url...</div>
    <input type="text" name="ta" id="ta" value="" style="width:65%" />
    <div>&#160;<br />Primary image...</div>
    <img src="data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=" name="image1" id="image1" style="max-width:85%" />
</body>
</html>
