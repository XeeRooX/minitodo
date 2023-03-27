async function main() {
    $("#add-task-group-btn").click(await AddTaskHandler);
    $("#cancel-btn").click(await CancelTaskAddHandler);
    $("#add-grpup-btn").click(await AddGroupHandler);
    $(".groups").on('click',".del-btn", DeleteItem);
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
    $(".groups").find("#" + idDel).parent().remove();
}

 function AddGroupToList(groupJson)
{
     var group = $(".group-example").clone();
     console.log(groupJson);
     group.find('.name-group').text(groupJson.name);
     group.find('.name-group').attr('id', groupJson.id);
     group.find('.del-btn').attr('id', groupJson.id);
    group.removeClass('d-none');
    group.removeClass('group-example');
    $(".groups").append(group);

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