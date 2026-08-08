// Repeatable "remind me X before" row editor. Serializes to a comma separated list of negative
// ISO-8601 durations (e.g. "-PT15M,-P1D") into the editor's hidden input. Scoped to each
// [data-alarm-editor] container instead of using global ids, so more than one could exist on a page.

document.addEventListener("DOMContentLoaded", function () {
  var editors = document.querySelectorAll("[data-alarm-editor]");

  editors.forEach(function (editor) {
    var hiddenInput = editor.querySelector("#Alarms") || editor.querySelector('input[name="Alarms"]');
    var rowsContainer = editor.querySelector("[data-alarm-rows]");
    var emptyText = editor.querySelector("[data-alarm-empty]");
    var addButton = editor.querySelector("[data-alarm-add]");
    var template = editor.querySelector("[data-alarm-template]");
    var maxAlarms = parseInt(editor.getAttribute("data-alarm-max"), 10) || 10;

    function parseValue(value) {
      if (!value) return [];
      return value
        .split(",")
        .map(function (part) { return part.trim(); })
        .filter(function (part) { return part.length > 0; })
        .map(function (part) {
          var match = /^-P(?:T(\d{1,4})([HM])|(\d{1,4})(D))$/.exec(part);
          if (!match) return null;
          var amount = match[1] || match[3];
          var unit = match[2] || match[4];
          return { amount: parseInt(amount, 10), unit: unit };
        })
        .filter(function (entry) { return entry !== null; });
    }

    function updateEmptyState() {
      var hasRows = rowsContainer.querySelectorAll("[data-alarm-row]").length > 0;
      emptyText.hidden = hasRows;
      addButton.hidden = rowsContainer.querySelectorAll("[data-alarm-row]").length >= maxAlarms;
    }

    function serialize() {
      var rows = rowsContainer.querySelectorAll("[data-alarm-row]");
      var parts = [];
      rows.forEach(function (row) {
        var amount = parseInt(row.querySelector("[data-alarm-amount]").value, 10);
        var unit = row.querySelector("[data-alarm-unit]").value;
        if (!amount || amount <= 0) return;
        parts.push(unit === "D" ? ("-P" + amount + "D") : ("-PT" + amount + unit));
      });
      hiddenInput.value = parts.join(",");
      updateEmptyState();
    }

    function addRow(amount, unit) {
      if (rowsContainer.querySelectorAll("[data-alarm-row]").length >= maxAlarms) return;

      var fragment = template.content.cloneNode(true);
      var row = fragment.querySelector("[data-alarm-row]");
      var amountInput = row.querySelector("[data-alarm-amount]");
      var unitSelect = row.querySelector("[data-alarm-unit]");
      var removeButton = row.querySelector("[data-alarm-remove]");

      amountInput.value = amount || 15;
      unitSelect.value = unit || "M";

      amountInput.addEventListener("input", serialize);
      unitSelect.addEventListener("change", serialize);
      removeButton.addEventListener("click", function () {
        row.remove();
        serialize();
      });

      rowsContainer.appendChild(row);
    }

    addButton.addEventListener("click", function () {
      addRow(15, "M");
      serialize();
    });

    // Seed rows from the hidden input's initial value
    parseValue(hiddenInput.value).forEach(function (entry) {
      addRow(entry.amount, entry.unit);
    });
    updateEmptyState();
  });
});
