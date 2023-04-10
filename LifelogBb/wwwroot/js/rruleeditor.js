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

var rruleEditorModalEndSelect = document.getElementById("rruleEditorModalEnd");
var rruleEditorModalUntilInput = document.getElementById("rruleEditorModalUntilInput");
var rruleEditorModalCountInput = document.getElementById("rruleEditorModalCountInput");


function updateCurrentRule() {
  var weekdays = [];
  var monthdays = [];

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
  } else if (rruleEditorModalFrequencySelect.value === "1") { // Monthly
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

  var rule = new rruleModule.RRule({
    freq: rruleEditorModalFrequencySelect.value,
    interval: rruleEditorModalFrequencySelect.value !== "0" ? rruleEditorModalIntervalInput.value : "",
    until: rruleEditorModalEndSelect.value === "on" && rruleEditorModalUntilInput.value != "" ? new Date(rruleEditorModalUntilInput.value) : null,
    count: rruleEditorModalEndSelect.value === "count" ? rruleEditorModalCountInput.value : null,
    byweekday: weekdays,
    bymonthday: monthdays,
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
  document.getElementById("rruleEditorModalIntervalInfo").innerText = newVal === "1" ? "Month(s)" : newVal === "2" ? "Week(s)" : "Day(s)";

  document.getElementById("rruleEditorModalIntervalContainer").hidden = newVal === "0"; // Hide on yearly
  document.getElementById("rruleEditorModalByDayContainer").hidden = newVal !== "2"; // Show only on weekly
  document.getElementById("rruleEditorModalMonthlyByContainer1").hidden = newVal !== "1"; // Show only on monthly
  document.getElementById("rruleEditorModalMonthlyByContainer2").hidden = newVal !== "1"; // Show only on monthly
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

  console.log(rule.options)
  console.log(rule.options.byweekday)
  console.log(rule.options.bynweekday)
  if (rule.options.freq === 2) { // Weekly
    var days = ["MO", "TU", "WE", "TH", "FR", "SA", "SU"];
    if (rule.options.byweekday !== null) {
      rruleEditorModalByDayInput.forEach(s => {
        s.checked = rule.options.byweekday.includes(days.indexOf(s.value));
      });
    }
  } else if (rule.options.freq === 1) { // Monthly
    if (rule.options.bymonthday !== null) {
      rruleEditorModalMonthlyByInputBYMONTHDAY.checked = true;
      rruleEditorModalMonthlyByInputBYSETPOS.checked = false;
      var byMonthDayStrings = rule.options.bymonthday.map(d => d.toString());
      for (var i = 0; i < rruleEditorModalByMonthDayInput.options.length; i++) {
        rruleEditorModalByMonthDayInput.options[i].selected = byMonthDayStrings.includes(rruleEditorModalByMonthDayInput.options[i].value);
      }
    } else if (rule.options.bynweekday !== null) { // byNweekday => [[0, 1], [1, -1], ...]
    // rruleEditorModalBySetPosInput
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
