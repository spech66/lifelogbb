// Replace all rules in elements tagged with class "rrule" with human readable text

var rruleModule = rrule;

document.addEventListener("DOMContentLoaded", function () {
  var rrules = document.getElementsByClassName("rrule");

  if (rrules.length < 1) return;

  for (var i = 0; i < rrules.length; i++) {
    var rruleElement = rrules[i];
    var rruleText = rruleElement.innerText;

    if (rruleText.length > 0) {
      var rrule = new rruleModule.rrulestr(rruleText);
      rruleElement.innerText = rrule.toText();
    }
  }
});
