<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <style>
        body {font: normal .84em 'segoe ui'; padding: .5em; padding-bottom: 2em}
        input, textarea {font: normal 1em 'segoe ui'}
    </style>
    <script src="http://code.jquery.com/jquery-1.12.4.min.js"></script>
    <script>
        function parseImage() {
            $("#imgpanel").css("height", $("#h").val());
            $("#imgpanel").css("width", $("#w").val());
            var transGif = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
            var url = "/thumbnail?mw=" + $("#w").val() + "&mh=" + $("#h").val() + "&url=" + encodeURIComponent($("#url").val());
            $("#serviceurl").text("http://services.ayespee.net" + url);
            $.ajax({
                url: url,
                async: false
            })
            .done(function (data) {
                $("#image0").attr("src", "data:image/jpeg;base64," + data);
                $("#image1").attr("src", $("#url").val());
                $("#ta").val(data);
            })
            .fail(function (xhr) {
                $("#image0").attr("src", transGif);
                $("#image1").attr("src", transGif);
                $("#ta").val("Request Status: " + xhr.status + " Status Text: " + xhr.statusText + " " + xhr.responseText);
            });
        }
    </script>
</head>
<body>
    <div>Enter an image url to parse...</div>
    <div>
        <input type="text" name="url" id="url" value="http://i.telegraph.co.uk/multimedia/archive/02780/weather-wave-ilfra_2780330k.jpg" style="width:65%" />
        <input type="button" name="submit" id="submit" value="Submit" onclick="parseImage()" style="height:1.8em" />
    </div>
    <div>&#160;<br />Thumbnail max dimensions...</div>
    <div>
         w <input type="text" name="w" id="w" value="240" style="display:inline;width:70px" />
         h <input type="text" name="h" id="h" value="180" style="display:inline;width:70px" /> 
    </div>
    <div>&#160;<br />Service...<br />
        <span id="serviceurl">[thumbnail service]</span>
    </div>
    <div>&#160;<br />Thumbnail image...</div>
    <div name="imgpanel" id="imgpanel" style="border: 1px solid #ddd; text-align:center; width: 240px; height: 180px;">
        <img src="data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=" name="image0" id="image0" />
    </div>
    <div>&#160;<br />Thumbnail base64 data...</div>
    <textarea name="ta" id="ta" style="width:85%;height:200px;"></textarea>
    <div>&#160;<br />Original image...</div>
    <img src="data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=" name="image1" id="image1" style="max-width:85%" />
</body>
</html>
