// Repeatable training plan set row editor. Serializes to a JSON array of
// { exercise, reps, weight, durationSeconds, notes } into the editor's hidden SetsJson input. An empty
// weight or duration field serializes to null rather than 0, because "no weight applies" (bodyweight,
// band, mobility work) is a distinct state from an actual zero. Scoped to each
// [data-plan-set-editor] container instead of using global ids, so more than one could exist on a page.
// Exercise autocomplete uses a plain <datalist> (see PlanSets.cshtml) instead of Tagify, since Tagify
// would need per-row re-initialization after each clone from the <template>.

document.addEventListener("DOMContentLoaded", function () {
  var editors = document.querySelectorAll("[data-plan-set-editor]");

  editors.forEach(function (editor) {
    var hiddenInput = editor.querySelector("#SetsJson") || editor.querySelector('input[name="SetsJson"]');
    var rowsContainer = editor.querySelector("[data-plan-set-rows]");
    var emptyText = editor.querySelector("[data-plan-set-empty]");
    var template = editor.querySelector("[data-plan-set-template]");

    function updateEmptyState() {
      var hasRows = rowsContainer.querySelectorAll("[data-plan-set-row]").length > 0;
      emptyText.hidden = hasRows;
    }

    // Blank stays blank: an empty numeric field is null, not 0.
    function optionalNumber(input) {
      var raw = input.value.trim();
      if (raw === "") return null;
      var value = parseFloat(raw);
      return isNaN(value) ? null : value;
    }

    function serialize() {
      var rows = rowsContainer.querySelectorAll("[data-plan-set-row]");
      var parts = [];
      rows.forEach(function (row) {
        parts.push({
          exercise: row.querySelector("[data-plan-set-exercise]").value.trim(),
          reps: parseInt(row.querySelector("[data-plan-set-reps]").value, 10) || 0,
          weight: optionalNumber(row.querySelector("[data-plan-set-weight]")),
          durationSeconds: optionalNumber(row.querySelector("[data-plan-set-duration]")),
          notes: row.querySelector("[data-plan-set-notes]").value.trim() || null
        });
      });
      hiddenInput.value = JSON.stringify(parts);
      updateEmptyState();
    }

    function addRow(data, afterRow) {
      var fragment = template.content.cloneNode(true);
      var row = fragment.querySelector("[data-plan-set-row]");
      var exerciseInput = row.querySelector("[data-plan-set-exercise]");
      var repsInput = row.querySelector("[data-plan-set-reps]");
      var weightInput = row.querySelector("[data-plan-set-weight]");
      var durationInput = row.querySelector("[data-plan-set-duration]");
      var notesInput = row.querySelector("[data-plan-set-notes]");

      exerciseInput.value = (data && data.exercise) || "";
      repsInput.value = (data && data.reps) || 10;
      weightInput.value = data && data.weight !== null && data.weight !== undefined ? data.weight : "";
      durationInput.value = data && data.durationSeconds !== null && data.durationSeconds !== undefined ? data.durationSeconds : "";
      notesInput.value = (data && data.notes) || "";

      [exerciseInput, repsInput, weightInput, durationInput, notesInput].forEach(function (input) {
        input.addEventListener("input", serialize);
      });

      row.querySelector("[data-plan-set-remove]").addEventListener("click", function () {
        row.remove();
        serialize();
      });

      row.querySelector("[data-plan-set-up]").addEventListener("click", function () {
        var prev = row.previousElementSibling;
        if (prev) rowsContainer.insertBefore(row, prev);
        serialize();
      });

      row.querySelector("[data-plan-set-down]").addEventListener("click", function () {
        var next = row.nextElementSibling;
        if (next) rowsContainer.insertBefore(next, row);
        serialize();
      });

      row.querySelector("[data-plan-set-duplicate]").addEventListener("click", function () {
        addRow({
          exercise: exerciseInput.value,
          reps: parseInt(repsInput.value, 10) || 0,
          weight: optionalNumber(weightInput),
          durationSeconds: optionalNumber(durationInput),
          notes: notesInput.value
        }, row);
        serialize();
      });

      if (afterRow && afterRow.nextSibling) {
        rowsContainer.insertBefore(row, afterRow.nextSibling);
      } else {
        rowsContainer.appendChild(row);
      }
    }

    editor.querySelector("[data-plan-set-add]").addEventListener("click", function () {
      addRow(null);
      serialize();
    });

    // Seed rows from the hidden input's initial JSON value.
    try {
      var initial = hiddenInput.value ? JSON.parse(hiddenInput.value) : [];
      initial.forEach(function (entry) { addRow(entry); });
    } catch (e) {
      // Malformed JSON: start from an empty set list rather than failing to render the page.
    }
    updateEmptyState();
  });
});
