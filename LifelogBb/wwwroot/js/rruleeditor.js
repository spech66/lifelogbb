var rruleModule = rrule;

var rruleInput = document.getElementById("RecurrenceRules");
var reccurenceRulesTextElement = document.getElementById("rruleEditorText");
var reccurenceRulesModalTextElement = document.getElementById("rruleEditorModalText");
var rruleEditorModalFrequencySelect = document.getElementById("rruleEditorModalFrequency");
var rruleEditorModalEndSelect = document.getElementById("rruleEditorModalEnd");

function updateCurrentRule() {
  var rule = new rruleModule.RRule({
    freq: rruleEditorModalFrequencySelect.value,
    until: rruleEditorModalEndSelect.value === "on" ? new Date(new Date().getFullYear() + 1, 1, 1) : null, // TODO: Get date from input
    count: rruleEditorModalEndSelect.value === "count" ? 10 : null,// TODO: Get count from input
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

document.addEventListener("DOMContentLoaded", function () {
  var currentRule = rruleInput.value === "" ? "FREQ=DAILY;INTERVAL=1" : rruleInput.value;
  console.log(currentRule);
  const rule = rruleModule.RRule.fromString(currentRule);
  console.log(rule.options);

  // Set initial values
  reccurenceRulesTextElement.innerText = rule.toText();
  reccurenceRulesModalTextElement.innerText = rule.toText();
  rruleEditorModalFrequencySelect.value = rule.options.freq;

  if (rule.options.until !== null) {
    rruleEditorModalEndSelect.value = "on";
  } else if (rule.options.count !== null) {
    rruleEditorModalEndSelect.value = "count";
  } else {
    rruleEditorModalEndSelect.value = "never";
  }
});

rruleEditorModalFrequencySelect.onchange = function () {
  updateCurrentRule();
}

rruleEditorModalEndSelect.onchange = function () {
  updateCurrentRule();
}
