//вставка изначальной даты в срок возврата кредита
$(document).ready(function() {
    const date = calcDate(1);
    $("#DateDepositFinish").html(date);
    $("#Deposit_DateDepositFinish").val(date);

    const startDate = getCurrentDate();
    $("#Deposit_DateDeposit").val(startDate);
});


//подписываемся на событие изменение размеров окна
$(document).ready(function() {
    $(window).on("resize",
        function() {
            //перешли в телефонное разрешение
            if ($(window).width() < 992) {
                //перекидываем блок в главный
                $("#data_right_column").appendTo("#main_form");

                //убираем центрирование и отступ с права
                $("#main_form").removeClass("justify-content-center");
                $("#left_column").removeClass("mr-5");

                //добавляем отображение в колонку и отступ внизу
                $("#main_form").addClass("flex-column");
                $("#left_column").addClass("mb-4");

                //выставляем ширину
                $("#left_column").css("width", "100%");
            } else {
                //перекидываем блок в второстепенный
                $("#data_right_column").appendTo("#right_column");

                //убираем отображение в колонку и отступ внизу
                $("#main_form").removeClass("flex-column");
                $("#left_column").removeClass("mb-4");

                //добавляем центрирование и отступ с права
                $("#main_form").addClass("justify-content-center");
                $("#left_column").addClass("mr-5");

                //выставляем ширину
                $("#left_column").css("width", "70%");
            }
        });
});


//пересчет процентов, финальной стоимости, вывод суммы крдита
function recalculationValues() {
    const amount = $("#Deposit_Amount").val();
    const term = $("#Deposit_TermDeposit").val();

    $("#procent_deposit").html((Number(amount) * 0.01 * Number(term)).toFixed(2));
    $("#final_Amount").html((Number(amount) + Number(amount) * 0.01 * Number(term)).toFixed(2));
    $("#display_deposit_Amount").html(amount);
}

//формирование даты с учетом добавленных дней
function calcDate(addDays) {
    const newDate = new Date();
    newDate.setDate(newDate.getDate() + Number(addDays));
    var dd = newDate.getDate();
    const mm = newDate.getMonth() + 1;
    const y = newDate.getFullYear();

    if (dd < 10) {
        dd = `0${dd}`;
    }

    const someFormattedDate = dd + "/" + mm + "/" + y;

    return someFormattedDate;
}

function getCurrentDate() {
    const newDate = new Date();
    newDate.setDate(newDate.getDate());
    var dd = newDate.getDate();
    const mm = newDate.getMonth() + 1;
    const y = newDate.getFullYear();

    if (dd < 10) {
        dd = `0${dd}`;
    }

    const someFormattedDate = dd + "/" + mm + "/" + y;

    return someFormattedDate;
}


//Смена данных в полях

//смена данных в поле суммы кредита
$("#Deposit_Amount").change(function() {
    var amount = this.value;

    if (amount < 50) {
        amount = 50;
        this.value = amount;
    } else if (amount > 20000) {
        amount = 20000;
        this.value = amount;
    }

    $("#range_deposit_amount").val(amount);
    $("#display_deposit_Amount").val(amount);

    recalculationValues();
});

//смена данных в поле срока кредита
$("#Deposit_TermDeposit").change(function() {
    var term = this.value;

    if (term < 1) {
        term = 1;
        this.value = term;
    } else if (term > 30) {
        term = 30;
        this.value = term;
    }

    $("#rangeTermDeposit").val(term);

    //пересчет данных
    recalculationValues();

    //pritn finish date for credit
    const someFormattedDate = calcDate(this.value);
    $("#DateDepositFinish").html(someFormattedDate);
    $("#Deposit_DateDepositFinish").val(someFormattedDate);
});


//Перетягивание слайдера выбора суммы кредита
$("#range_deposit_amount").on("input",
    function() {
        $("#Deposit_Amount").val($(this).val());
        $("#display_deposit_Amount").html($(this).val());

        recalculationValues();
    });


//Перетягивание слайдера выбора срока кредита
$("#rangeTermDeposit").on("input",
    function() {
        $("#Deposit_TermDeposit").val($(this).val());

        //pritn finish date for credit
        const someFormattedDate = calcDate($(this).val());
        $("#DateDepositFinish").html(someFormattedDate);
        $("#Deposit_DateDepositFinish").val(someFormattedDate);

        recalculationValues();
    });