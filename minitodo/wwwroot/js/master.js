async function main() {
    $("#add-task-group-btn").click(await AddTaskHandler);
    $("#cancel-btn").click(await CancelTaskAddHandler);
    $("#add-grpup-btn").click(await AddGroupHandler);
    $(".groups").on('click', ".edit-btn", await EditNameGroup);
    $(".groups").on('click', ".save-edit-btn", await SaveEditGroup);
    $(".groups").on('click', ".del-btn", DeleteItem);
    $(".groups").on('click', '.ref-group-item', SelectItem)
    $(".fav-tasks-btn").click(await FavoriteShow);
    await GetAllGroups();
    await SetInfoUser();
}

async function SetInfoUser() {
    $.post("User/GetAuthoriseInfo")
        .done(function (data) {
            console.log(data.name);
            $('.user-name').text(data.name + " " + data.surname);
            $('.user-email').text(data.email);
        }).fail(function (xhr) {
            alert("error: " + xhr.responseText)
        });
}

async function FavoriteShow() {

    $(".group-name").text("Избранные задачи");
    $(".group-name").removeClass("d-none");
    $(".new-task-input").addClass("d-none");
    $(".conf-task-drop").addClass("d-none");
    //conf-task-drop

    ClearTasks();

    $.ajax({
        url: "Task/GetFavorited",
        type: "GET",
        contentType: "application/json; charset=utf-8",
    }).done(function (data) {
        console.log("Ok favorited!", data);

        if (data.length == 0) {
            PrintFavoritedEmptyMsg();
        } else {
            $(".msg-body").addClass("d-none");
        }

        for (let i = 0; i < data.length; i++) {
            PrintNotConfTasks(data[i]);
        }
    }).fail(function (xhr) {
        alert(xhr.responseText);
    });
} 


function PrintFavoritedEmptyMsg() {
    $(".message").text("Список избранных задач пуст");
    $(".msg-body").removeClass("d-none");
}

function PrintGroupEmptyMsg() {
    $(".message").text('Список задач пуст. Введите задачи в поле выше и нажмите кнопку "Добавить"');
    $(".msg-body").removeClass("d-none");
}


async function SelectItem() {
    //var idGroup = $(this).parent().attr("id");
    var idGroup = $(this).closest('.group-item').attr("id");


    var groupName = $(this).text();
    $(".group-name").removeClass("d-none");

    $(".new-task-input").removeClass("d-none");
    $(".conf-task-drop").removeClass("d-none");
    $(".group-name").text(groupName);
    $(".group-name").attr("id", idGroup);
    $(".name-task-input").val('');
    ClearTasks();


    // Получение и вывод невыполненных задач
    $.ajax({
        url: "Task/GetNotConfirmed",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            GroupId: Number(idGroup)
        }) 
    }).done(function (data) {
        if (data.length == 0) {
            PrintGroupEmptyMsg();
        } else {
            $(".msg-body").addClass("d-none");
        }

        for (let i = 0; i < data.length; i++) {
            PrintNotConfTasks(data[i]);
        }
    }).fail(function (xhr) {
        alert(xhr.responseText);
    });

    // Получение и вывод выполненных задач
    $.ajax({
        url: "Task/GetConfirmed",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            GroupId: Number(idGroup)
        })
    }).done(function (data) {
        console.log("Ok!!!!!", data);
        for (let i = 0; i < data.length; i++) {
            PrintConfTasks(data[i]);
        }
    }).fail(function (xhr) {
        alert(xhr.responseText);
    });



    //$.post("Task/GetNotConfirmed", { GroupId: Number(idGroup)}).done(function (data, status) {
    //    console.log("Ok get noconf tasks");
    //    console.log(data);
    //});

    console.log(idGroup);
}
function ClearTasks() {
    var temp = $(".noconf-task-temp").clone();
    var parent = $(".noconf-task-temp").parent();
    parent.children('li').remove();
    parent.append(temp);

    var temp2 = $(".conf-task-temp").clone();
    var parent2 = $(".conf-task-temp").parent();
    parent2.children('li').remove();
    parent2.append(temp2);
}

function PrintConfTasks(data) {
    var task = $(".conf-task-temp").clone();

    task.attr('id', data.id);
    task.find(".task-label").text(data.description);
    task.removeClass("conf-task-temp");
    task.removeClass("d-none");
    task.addClass("list-group-item");

    $(".conf-task-list").append(task);
}

function PrintNotConfTasks(data) {
    var task = $(".noconf-task-temp").clone();

    task.attr('id', data.id);
    task.find(".task-label").text(data.description);
    var starState = data.isFavorite;
    SetStarState(task, starState);

    task.removeClass("noconf-task-temp");
    task.removeClass("d-none");
    task.addClass("list-group-item");

    $(".noconf-task-list").append(task);
}

function SetStarState(task, state) {
    var star = task.find(".star-toggle");

    console.log("state", state);
    if (state === true) {
        if (star.hasClass("favorited") == false) {
            console.log("asdasdsa");
            star.addClass("favorited");
            star.children().attr("src", "/png/Star.png");
        }
    } else {     
        if (star.hasClass("favorited") == true) {
            console.log("Tut!!!");
            star.removeClass("favorited");
            star.children().attr("src", "/png/unStar.png");
        }
    }
}

async function AddGroupHandler() {
    var groupName = $("#name-input").val();
    var answer = $.post("Group/Create", { nameGroup: groupName })
        .done(function (data, statusText) {
            console.log("ok " + groupName);
            console.log(data);
            AddGroupToList(data);
        }).fail(function (xhr, textStatus, errorThrown) { alert("error: " + xhr.responseText); })
        ;

  //  alert(groupName);
}

function DeleteItem() {
    var idDel = $(this).attr("id");
    console.log("idDel", idDel);
    console.log('ddfgdg');
    $.post("Group/Delete", { id: idDel })
        .done(function (data, statusText) {
            console.log("ok " + idDel);
        }).fail(function (xhr, textStatus, errorThrown) { alert("error: " + xhr.responseText); })
        ;
    $(".groups").children("#" + idDel).remove();
}

 function AddGroupToList(groupJson)
{
     var group = $(".group-example").clone();
     console.log(groupJson);
     group.find('.name-group').text(groupJson.name);
     group.attr('id', groupJson.id);
     group.find('.name-group').attr('id', groupJson.id);
     group.find('.del-btn').attr('id', groupJson.id);
     group.find('.edit-panel').attr('id', groupJson.id);
     group.attr('id', groupJson.id);
    group.removeClass('d-none');
    group.removeClass('group-example');
    $(".groups").append(group);

}

async function EditNameGroup() {
    $(this).parent().addClass("d-none");
    $(this).parent().next().removeClass("d-none");
    $(this).parent().next().children('.form-control').val($(this).parent().find(".name-group").text());
}
async function returnToDefaultPanel(name, id) {
    var dom = $('#' + id);
    dom.children('.default-panel').removeClass("d-none");
    dom.children('.default-panel').children('.name-group').text(name);
    dom.children('.edit-panel').addClass("d-none");
}
async function SaveEditGroup() {
    var newNameGroup = $(this).closest(".edit-panel").children('.form-control').val();
    var idGroup = $(this).closest(".edit-panel").attr("id");
    console.log(idGroup);

    $.post("Group/Edit", { id: idGroup, nameGroup: newNameGroup })
        .done(function (data, statusText) {
            console.log("ok " + data);
            returnToDefaultPanel(data, idGroup);
        }).fail(function (xhr, textStatus, errorThrown) { alert("error: " + xhr.responseText); })
        ;
}
async function GetAllGroups() {
    console.log('sdfsf');
    $.get("Group/Read")
        .done(function (data, statusText) {
            if (data.length == 0) {
                ShowNoGroupsMsg();
            } 
            console.log(data);
            for (let i = 0; i < data.length; i++) {
                AddGroupToList(data[i]);
            }
        }).fail(function (xhr, textStatus, errorThrown) { alert("error: " + xhr.responseText); })
        ;
}

function ShowNoGroupsMsg() {
    $(".message").text("Список групп пуст. Создайте новую.");
    $(".msg-body").removeClass("d-none");
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