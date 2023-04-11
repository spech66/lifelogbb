var rruleModule = rrule;

var rruleInput = document.getElementById("RecurrenceRules");
var reccurenceRulesTextElement = document.getElementById("rruleEditorText");
var reccurenceRulesModalTextElement = document.getElementById("rruleEditorModalText");

var rruleEditorModalFrequencySelect = document.getElementById("rruleEditorModalFrequency");
var rruleEditorModalIntervalInput = document.getElementById("rruleEditorModalIntervalInput");
var rruleEditorModalByDayInput = document.getElementsByName("rruleEditorModalByDayInput"); // Group!
var rruleEditorModalMonthlyByInput = document.getElementsByName("rruleEditorModalMonthlyByInput"); // Group!
var rruleEditorModalByMonthDayInput = document.getElementById("rruleEditorModalByMonthDayInput");
var rruleEditorModalBySetPosInput = document.getElementById("rruleEditorModalBySetPosInput");
var rruleEditorModalByMonthInput = document.getElementById("rruleEditorModalByMonthInput");

var rruleEditorModalEndSelect = document.getElementById("rruleEditorModalEnd");
var rruleEditorModalUntilInput = document.getElementById("rruleEditorModalUntilInput");
var rruleEditorModalCountInput = document.getElementById("rruleEditorModalCountInput");


function updateCurrentRule() {
  var weekdays = [];
  var monthdays = [];
  var yearmonths = [];

  // Stupid conversion because library doesn't support array of days/ints
  if (rruleEditorModalFrequencySelect.value === "2") { // Weekly
    var dayInputs = Array.prototype.slice.call(rruleEditorModalByDayInput, 0);
    dayInputs.forEach(s => {
      if (!s.checked) return;
      if (s.value === "MO") weekdays.push(rruleModule.RRule.MO);
      else if (s.value === "TU") weekdays.push(rruleModule.RRule.TU);
      else if (s.value === "WE") weekdays.push(rruleModule.RRule.WE);
      else if (s.value === "TH") weekdays.push(rruleModule.RRule.TH);
      else if (s.value === "FR") weekdays.push(rruleModule.RRule.FR);
      else if (s.value === "SA") weekdays.push(rruleModule.RRule.SA);
      else if (s.value === "SU") weekdays.push(rruleModule.RRule.SU);
    });
  }

  if (rruleEditorModalFrequencySelect.value === "1" || rruleEditorModalFrequencySelect.value === "0") { // Monthly and Yearly
    if (document.getElementById("rruleEditorModalMonthlyByInputBYMONTHDAY").checked) { // ex: 1, 4, 9, ...
      for (var i = 0; i < rruleEditorModalByMonthDayInput.selectedOptions.length; i++) {
        monthdays.push(rruleEditorModalByMonthDayInput.selectedOptions[i].value);
      }
    } else if (document.getElementById("rruleEditorModalMonthlyByInputBYSETPOS").checked) { // ex: 1MO, -1FR, 2TU, ...
      for (var i = 0; i < rruleEditorModalBySetPosInput.selectedOptions.length; i++) {
        // add entries in the format of "1MO", "-1FR", "2TU", ... to the weekdays array
        var day = rruleEditorModalBySetPosInput.selectedOptions[i].value;
        var dayNumber = day.substring(0, day.length - 2);
        var dayName = day.substring(day.length - 2, day.length);
        if (dayName === "MO") weekdays.push(rruleModule.RRule.MO.nth(parseInt(dayNumber)));
        else if (dayName === "TU") weekdays.push(rruleModule.RRule.TU.nth(parseInt(dayNumber)));
        else if (dayName === "WE") weekdays.push(rruleModule.RRule.WE.nth(parseInt(dayNumber)));
        else if (dayName === "TH") weekdays.push(rruleModule.RRule.TH.nth(parseInt(dayNumber)));
        else if (dayName === "FR") weekdays.push(rruleModule.RRule.FR.nth(parseInt(dayNumber)));
        else if (dayName === "SA") weekdays.push(rruleModule.RRule.SA.nth(parseInt(dayNumber)));
        else if (dayName === "SU") weekdays.push(rruleModule.RRule.SU.nth(parseInt(dayNumber)));
      }
    }
  }

  if (rruleEditorModalFrequencySelect.value === "0") { // Yearly
    for (var i = 0; i < rruleEditorModalByMonthInput.selectedOptions.length; i++) {
      yearmonths.push(rruleEditorModalByMonthInput.selectedOptions[i].value);
    }
  }

  var rule = new rruleModule.RRule({
    freq: rruleEditorModalFrequencySelect.value,
    interval: rruleEditorModalIntervalInput.value !== "" || rruleEditorModalIntervalInput.value !== "1" ? rruleEditorModalIntervalInput.value : "",
    until: rruleEditorModalEndSelect.value === "on" && rruleEditorModalUntilInput.value != "" ? new Date(rruleEditorModalUntilInput.value) : null,
    count: rruleEditorModalEndSelect.value === "count" ? rruleEditorModalCountInput.value : null,
    byweekday: weekdays,
    bymonthday: monthdays,
    bymonth: yearmonths,
  });

  console.log(rule.toString());
  console.log(rule.toText());

  // Update elements with new rule
  rruleInput.value = rule.toString();
  reccurenceRulesTextElement.innerText = rule.toText();
  reccurenceRulesModalTextElement.innerText = rule.toText();

  // console.log(rule.all());
  // console.log(rule.between(new Date(new Date().getFullYear(), 1, 1), new Date(new Date().getFullYear() + 1, 1, 1), true));
}

function updateFrequencyControls() {
  var newVal = rruleEditorModalFrequencySelect.value;
  switch (newVal) {
    case "0":
      document.getElementById("rruleEditorModalIntervalInfo").innerText = "Year(s)";
      break;
    case "1":
      document.getElementById("rruleEditorModalIntervalInfo").innerText = "Month(s)";
      break;
    case "2":
      document.getElementById("rruleEditorModalIntervalInfo").innerText = "Week(s)";
      break;
    case "3":
      document.getElementById("rruleEditorModalIntervalInfo").innerText = "Day(s)";
      break;
  }

  document.getElementById("rruleEditorModalByDayContainer").hidden = newVal !== "2"; // Show only on weekly
  document.getElementById("rruleEditorModalMonthlyByContainer1").hidden = (newVal !== "1" && newVal !== "0"); // Show only on monthly and yearly
  document.getElementById("rruleEditorModalMonthlyByContainer2").hidden = (newVal !== "1" && newVal !== "0"); // Show only on monthly and yearly
  document.getElementById("rruleEditorModalYearlyByContainer").hidden = newVal !== "0"; // Show only on yearly
}

function updateEndControls() {
  if (rruleEditorModalEndSelect.value === "on") {
    document.getElementById("rruleEditorModalCountContainer").hidden = true;
    document.getElementById("rruleEditorModalUntilContainer").hidden = false;
  } else if (rruleEditorModalEndSelect.value === "count") {
    document.getElementById("rruleEditorModalCountContainer").hidden = false;
    document.getElementById("rruleEditorModalUntilContainer").hidden = true;
  } else {
    document.getElementById("rruleEditorModalCountContainer").hidden = true;
    document.getElementById("rruleEditorModalUntilContainer").hidden = true;
  }
}

document.addEventListener("DOMContentLoaded", function () {
  var currentRule = rruleInput.value === "" ? "FREQ=DAILY;INTERVAL=1" : rruleInput.value;
  const rule = rruleModule.RRule.fromString(currentRule);

  // Set initial values
  reccurenceRulesTextElement.innerText = rule.toText();
  reccurenceRulesModalTextElement.innerText = rule.toText();
  rruleEditorModalFrequencySelect.value = rule.options.freq;
  rruleEditorModalIntervalInput.value = rule.options.interval;
  rruleEditorModalUntilInput.value = rule.options.until;
  rruleEditorModalCountInput.value = rule.options.count;

  var days = ["MO", "TU", "WE", "TH", "FR", "SA", "SU"];
  if (rule.options.freq === 2) { // Weekly
    if (rule.options.byweekday !== null) {
      rruleEditorModalByDayInput.forEach(s => {
        s.checked = rule.options.byweekday.includes(days.indexOf(s.value));
      });
    }
  }

  if (rule.options.freq === 1 || rule.options.freq === 0) { // Monthly and Yearly
    if (rule.options.bymonthday !== null && rule.options.bymonthday.length > 0) {
      rruleEditorModalMonthlyByInputBYMONTHDAY.checked = true;
      rruleEditorModalMonthlyByInputBYSETPOS.checked = false;
      var byMonthDayStrings = rule.options.bymonthday.map(d => d.toString());
      for (var i = 0; i < rruleEditorModalByMonthDayInput.options.length; i++) {
        rruleEditorModalByMonthDayInput.options[i].selected = byMonthDayStrings.includes(rruleEditorModalByMonthDayInput.options[i].value);
      }
    } else if (rule.options.bynweekday !== null && rule.options.bynweekday.length > 0) { // byNweekday => [[0, 1], [1, -1], ...]
      rruleEditorModalMonthlyByInputBYMONTHDAY.checked = false;
      rruleEditorModalMonthlyByInputBYSETPOS.checked = true;
      var bySetPosStrings = rule.options.bynweekday.map(d => d[1].toString() + days[d[0]]); // [0, 2] => "2MO"
      for (var i = 0; i < rruleEditorModalBySetPosInput.options.length; i++) {
        rruleEditorModalBySetPosInput.options[i].selected = bySetPosStrings.includes(rruleEditorModalBySetPosInput.options[i].value);
      }
    }
  }

  if (rule.options.freq === 0) { // Yearly
    if (rule.options.bymonth !== null && rule.options.bymonth.length > 0) {
      var byMonthStrings = rule.options.bymonth.map(d => d.toString());
      for (var i = 0; i < rruleEditorModalByMonthInput.options.length; i++) {
        rruleEditorModalByMonthInput.options[i].selected = byMonthStrings.includes(rruleEditorModalByMonthInput.options[i].value);
      }
    }
  }

  updateFrequencyControls();

  if (rule.options.until !== null) {
    rruleEditorModalEndSelect.value = "on";
  } else if (rule.options.count !== null) {
    rruleEditorModalEndSelect.value = "count";
  } else {
    rruleEditorModalEndSelect.value = "never";
  }
  updateEndControls();
});

rruleEditorModalFrequencySelect.onchange = function () {
  updateFrequencyControls();
  updateCurrentRule();
}

rruleEditorModalIntervalInput.onchange = function () {
  updateCurrentRule();
}

rruleEditorModalByDayInput.forEach(s => {
  s.onchange = function () {
    updateCurrentRule();
  }
});

rruleEditorModalByMonthInput.onchange = function () {
  updateCurrentRule();
}

rruleEditorModalMonthlyByInput.forEach(s => {
  s.onselectionchange = function () {
    updateCurrentRule();
  }
});

rruleEditorModalByMonthDayInput.onchange = function () {
  updateCurrentRule();
}

rruleEditorModalBySetPosInput.onchange = function () {
  updateCurrentRule();
}

rruleEditorModalEndSelect.onchange = function () {
  updateEndControls();
  updateCurrentRule();
}

rruleEditorModalUntilInput.onchange = function () {
  updateCurrentRule();
}

rruleEditorModalCountInput.onchange = function () {
  updateCurrentRule();
}
