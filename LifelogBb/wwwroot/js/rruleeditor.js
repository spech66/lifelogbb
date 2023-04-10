var rruleModule = rrule;

var rruleInput = document.getElementById("RecurrenceRules");
var reccurenceRulesTextElement = document.getElementById("rruleEditorText");
var reccurenceRulesModalTextElement = document.getElementById("rruleEditorModalText");

var rruleEditorModalFrequencySelect = document.getElementById("rruleEditorModalFrequency");
var rruleEditorModalIntervalInput = document.getElementById("rruleEditorModalIntervalInput");
var rruleEditorModalByDayInput = document.getElementsByName("rruleEditorModalByDayInput"); // Group!

var rruleEditorModalEndSelect = document.getElementById("rruleEditorModalEnd");
var rruleEditorModalUntilInput = document.getElementById("rruleEditorModalUntilInput");
var rruleEditorModalCountInput = document.getElementById("rruleEditorModalCountInput");


function updateCurrentRule() {
  // Stupid conversion because library doesn't support array of days/ints
  var dayInputs = Array.prototype.slice.call(rruleEditorModalByDayInput, 0);
  var weekdays = [];
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

  var rule = new rruleModule.RRule({
    freq: rruleEditorModalFrequencySelect.value,
    interval: rruleEditorModalFrequencySelect.value !== "0" ? rruleEditorModalIntervalInput.value : "",
    until: rruleEditorModalEndSelect.value === "on" && rruleEditorModalUntilInput.value != "" ? new Date(rruleEditorModalUntilInput.value) : null,
    count: rruleEditorModalEndSelect.value === "count" ? rruleEditorModalCountInput.value : null,
    byweekday: weekdays,
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
  if (rule.options.byweekday !== null) {
    rruleEditorModalByDayInput.forEach(s => {
      s.checked = rule.options.byweekday.includes(days.indexOf(s.value));
    });
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
