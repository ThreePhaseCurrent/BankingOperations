//вставка изначальной даты в срок возврата кредита
$(document).ready(function() {
    const date = calcDate(1);
    $("#DateCreditFinish").html(date);
    $("#Credit_DateCreditFinish").val(date);

    const startDate = getCurrentDate();
    $("#Credit_DateCredit").val(startDate);
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
    const amount = $("#Credit_Amount").val();
    const term = $("#Credit_TermCredit").val();

    $("#procent_credit").html((Number(amount) * 0.01 * Number(term)).toFixed(2));
    $("#final_Amount").html((Number(amount) + Number(amount) * 0.01 * Number(term)).toFixed(2));
    $("#display_Amount").html(amount);
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
$("#Credit_Amount").change(function() {
    var amount = this.value;

    if (amount < 50) {
        amount = 50;
        this.value = amount;
    } else if (amount > 20000) {
        amount = 20000;
        this.value = amount;
    }

    $("#range_amount").val(amount);

    recalculationValues();
});

//смена данных в поле срока кредита
$("#Credit_TermCredit").change(function() {
    var term = this.value;

    if (term < 1) {
        term = 1;
        this.value = term;
    } else if (term > 30) {
        term = 30;
        this.value = term;
    }

    $("#rangeTermCredit").val(term);

    //пересчет данных
    recalculationValues();

    //pritn finish date for credit
    const someFormattedDate = calcDate(this.value);
    $("#DateCreditFinish").html(someFormattedDate);
    $("#Credit_DateCreditFinish").val(someFormattedDate);
});


//Перетягивание слайдера выбора суммы кредита
$("#range_amount").on("input",
    function() {
        $("#Credit_Amount").val($(this).val());
        $("#display_Amount").html($(this).val());

        recalculationValues();
    });


//Перетягивание слайдера выбора срока кредита
$("#rangeTermCredit").on("input",
    function() {
        $("#Credit_TermCredit").val($(this).val());

        //pritn finish date for credit
        const someFormattedDate = calcDate($(this).val());
        $("#DateCreditFinish").html(someFormattedDate);
        $("#Credit_DateCreditFinish").val(someFormattedDate);

        recalculationValues();
    });