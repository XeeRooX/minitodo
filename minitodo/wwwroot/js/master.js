async function main() {
    $("#add-task-group-btn").click(await AddTaskHandler);
}

async function AddTaskHandler() {
    $("#cancel-btn-div").removeClass("d-none");
    $("#name-input-group").removeClass("d-none");
    $("#add-task-btn-div").addClass("d-none");
}
main();