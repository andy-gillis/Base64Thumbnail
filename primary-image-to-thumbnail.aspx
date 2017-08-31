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
                var url2 = "/thumbnail?mw=" + $("#w").val() + "&mh=" + $("#h").val() + "&url=" + encodeURIComponent($("#ta").val());
                $("#serviceurl2").text("http://services.ayespee.net" + url2);
                $.ajax({
                    url: url2,
                    async: false
                })
                .done(function (xhrdata) {
                    $("#image0").attr("src", "data:image/jpeg;base64," + xhrdata);
                })
                .fail(function (xhr) {
                    $("#image0").attr("src", transGif);
                    $("#ta").val("Request Status: " + xhr.status + " Status Text: " + xhr.statusText + " " + xhr.responseText);
                });
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
        <input type="text" name="url" id="url" value="https://jfxgillis.newsvine.com/_news/2016/10/08/36193025-its-alright-ma-im-only-bleeding-youtube" style="width:65%" />
        <input type="button" name="submit" id="submit" value="Submit" onclick="parseWebsite()" style="height:1.8em" />
    </div>
     <div>&#160;<br />Thumbnail max dimensions...</div>
    <div>
         w <input type="text" name="w" id="w" value="240" style="display:inline;width:70px" />
         h <input type="text" name="h" id="h" value="180" style="display:inline;width:70px" /> 
    </div>
    <div>&#160;<br />Calling: <span name="serviceurl" id="serviceurl">[primary-image service]</span></div>
    <div>&#160;<br />Calling: <span name="serviceurl2" id="serviceurl2">[thumbnail service]</span></div>
    <div>&#160;<br />Primary image url...</div>
    <input type="text" name="ta" id="ta" value="" style="width:65%" />
    <div>&#160;<br />Thumbnail image...</div>
    <div name="imgpanel" id="imgpanel" style="border: 1px solid #ddd; text-align:center; width: 240px; height: 180px;">
        <img src="data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=" name="image0" id="image0" />
    </div>
    <div>&#160;<br />Original image...</div>
    <img src="data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=" name="image1" id="image1" style="max-width:85%" />
</body>
</html>
