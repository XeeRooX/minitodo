function main() {
    $(".noconf-task-list").on('click', ".star-toggle", StarClick);
    $(".noconf-task-list").on('click', ".noconf-task-chbx", ConfirmClick);
    $(".conf-task-list").on('click', ".task-delete-btn", DeleteTaskHandler);
    $(".task-create-btn").click(TaskCreateHandler);
}

function DeleteTaskHandler() {
    var taskId = $(this).closest(".conf-task").attr("id");
    var task = $(this).closest(".conf-task");

    $.ajax({
        url: "Task/Delete",
        type: "DELETE",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            TaskId: Number(taskId),
        })
    }).done(function (data) {
        task.remove();
    }).fail(function (xhr, textStatus) {
        alert(xhr.responseText);
    });
}

function ConfirmClick() {
    var taskId = $(this).closest(".noconf-task").attr("id");
    var descr = $(this).parent().children(".task-label").text();
    var task = $(this).closest(".noconf-task");


    $.ajax({
        url: "Task/SetConfirmed",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            TaskId: Number(taskId),
        })
    }).done(function (data) {
        var temp_data = { id: taskId, description: descr };

        PrintConfTasks(temp_data);
        task.remove();        
    }).fail(function (xhr, textStatus) {
        alert(xhr.responseText);
    });

}

function TaskCreateHandler() {
    var groupId = $(".group-name").attr("id");
    var taskTitle = $(".name-task-input").val();

    if (!taskTitle) {
        alert("Введите наименование задачи");
        return;
    }

    $.ajax({
        url: "Task/Create",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            TaskTitle: taskTitle,
            GroupId: Number(groupId)
        })
    }).done(function (data) {
        $(".name-task-input").val('');
        UpdateNotConfTasks(data);

    }).fail(function (xhr, textStatus) {
        alert(xhr.responseText);
    });

    $(".msg-body").addClass("d-none");
}

function UpdateNotConfTasks(data) {
    var task = $(".noconf-task-temp").clone();

    task.attr('id', data.taskId);
    task.find(".task-label").text(data.description);

    task.removeClass("noconf-task-temp");
    task.removeClass("d-none");
    task.addClass("list-group-item");

    $(".noconf-task-list").append(task);
}

function StarClick() {
    if ($(this).hasClass("favorited") === true) {
        $(this).removeClass("favorited");
        $(this).children().attr("src", "/png/unStar.png");
    }
    else {
        $(this).addClass("favorited");
        $(this).children().attr("src", "/png/Star.png");
        
    }
    var taskId = $(this).closest(".noconf-task").attr("id");
     ChangeStarState(taskId);
}

function ChangeStarState(taskId) {
    $.ajax({
        url: "Task/SetFavorited",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            TaskId: Number(taskId)
        })
    }).fail(function (xhr, textStatus) {
        alert(xhr.responseText);
    });
}

main();
