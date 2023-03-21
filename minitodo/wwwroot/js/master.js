async function main() {
    $("#add-task-group-btn").click(await AddTaskHandler);
    $("#cancel-btn").click(await CancelTaskAddHandler);
    $("#add-grpup-btn").click(await AddGroupHandler);
}

async function AddGroupHandler() {
    var groupName = $("#name-input").val();
    alert(groupName);
}

async function CancelTaskAddHandler() {
    $("#add-task-btn-div").removeClass("d-none");
    $("#name-input-group").addClass("d-none");
    $("#cancel-btn-div").addClass("d-none");
}

async function AddTaskHandler() {
    $("#name-input").val("");
    $("#cancel-btn-div").removeClass("d-none");
    $("#name-input-group").removeClass("d-none");
    $("#add-task-btn-div").addClass("d-none");
}
main();