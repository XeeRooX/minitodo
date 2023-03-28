async function main() {
    $("#add-task-group-btn").click(await AddTaskHandler);
    $("#cancel-btn").click(await CancelTaskAddHandler);
    $("#add-grpup-btn").click(await AddGroupHandler);
    $(".groups").on('click', ".del-btn", DeleteItem);
    $(".groups").on('click', ".edit-btn", await EditNameGroup);
    $(".groups").on('click', ".save-edit-btn", await SaveEditGroup);
    await GetAllGroups();
}

async function AddGroupHandler() {
    var groupName = $("#name-input").val();
    var answer = $.post("Group/Create", { nameGroup: groupName })
        .done(function (data, statusText) {
            console.log("ok " + groupName);
            console.log(data);
            $("#group-name").text(groupName);
            AddGroupToList(data);
        }).fail(function (xhr, textStatus, errorThrown) { alert("error: " + xhr.responseText); })
        ;

  //  alert(groupName);
}

function DeleteItem() {
    var idDel = $(this).attr("id");
    console.log(idDel);
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
            console.log(data);
            for (let i = 0; i < data.length; i++) {
                AddGroupToList(data[i]);
            }
        }).fail(function (xhr, textStatus, errorThrown) { alert("error: " + xhr.responseText); })
        ;
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