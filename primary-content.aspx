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
            var url = "/primarycontent?url=" + encodeURIComponent($("#url").val());
            var transGif = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
            $("#serviceurl1").text("http://services.ayespee.net" + url);
            $("#image").attr("src", transGif);
            $("#primaryHeading").val("");
            $.ajax({
                url: url,
                async: false
            })
            .done(function (data) {
                $("#primaryHeading").text(data.primaryHeading);
                var url2 = "/thumbnail?mw=" + $("#w").val() + "&mh=" + $("#h").val() + "&url=" + encodeURIComponent(data.primaryImageUrl);
                $("#serviceurl2").text("http://services.ayespee.net" + url2);
                $.ajax({
                    url: url2,
                    async: false
                })
                .done(function (xhrdata) {
                    $("#image").attr("src", "data:image/jpeg;base64," + xhrdata);
                })
                .fail(function (xhr) {
                    $("#image").attr("src", transGif);
                });
            })
        }
    </script>
</head>
<body>
    <div>Enter webpage url and image dimensions to render primary content...</div>
    <div>
        <input type="text" id="url" value="http://www.cnn.com/2017/09/01/health/utah-nurse-arrest-police-video/index.html" style="width:65%" />
        <input type="button" id="submit" value="Submit" onclick="parseWebsite()" style="height:1.8em" />
    </div>
    <div style="padding-top: 12px">
        <label>w:</label>
        <input type="text" name="w" id="w" value="400" style="display:inline; width:3em" />
        <label>h:</label>
        <input type="text" name="h" id="h" value="300" style="display:inline; width:3em" />
    </div>
    <div>&#160;<br />Services...<br />
        <span id="serviceurl1">[primary-content service]</span><br />
        <span id="serviceurl2">[thumbnail service]</span>
    </div>
    <div>&#160;<br />Primary-image and heading...</div>
    <div id="imgpanel" style="border: 1px solid #ddd; text-align:center; min-width: 400px; min-height: 300px; display: inline-block;">
        <figure>
            <img src="data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=" id="image" />
            <figcaption id="primaryHeading" style="text-align: center; font-weight: bold"></figcaption>
        </figure>
    </div>
</body>
</html>
