var rruleModule = rrule;

var rruleInput = document.getElementById("RecurrenceRules");
var reccurenceRulesTextElement = document.getElementById("rruleEditorText");
var reccurenceRulesModalTextElement = document.getElementById("rruleEditorModalText");

var rruleEditorModalFrequencySelect = document.getElementById("rruleEditorModalFrequency");
var rruleEditorModalIntervalInput = document.getElementById("rruleEditorModalIntervalInput");

var rruleEditorModalEndSelect = document.getElementById("rruleEditorModalEnd");
var rruleEditorModalUntilInput = document.getElementById("rruleEditorModalUntilInput");
var rruleEditorModalCountInput = document.getElementById("rruleEditorModalCountInput");


function updateCurrentRule() {
  var rule = new rruleModule.RRule({
    freq: rruleEditorModalFrequencySelect.value,
    interval: rruleEditorModalFrequencySelect.value !== "0" ? rruleEditorModalIntervalInput.value : "",
    until: rruleEditorModalEndSelect.value === "on" && rruleEditorModalUntilInput.value != "" ? new Date(rruleEditorModalUntilInput.value) : null,
    count: rruleEditorModalEndSelect.value === "count" ? rruleEditorModalCountInput.value : null,
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
  console.log(currentRule);
  const rule = rruleModule.RRule.fromString(currentRule);
  console.log(rule.options);

  // Set initial values
  reccurenceRulesTextElement.innerText = rule.toText();
  reccurenceRulesModalTextElement.innerText = rule.toText();
  rruleEditorModalFrequencySelect.value = rule.options.freq;
  rruleEditorModalUntilInput.value = rule.options.until;
  rruleEditorModalCountInput.value = rule.options.count;

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
