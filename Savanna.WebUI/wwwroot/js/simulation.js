const config = JSON.parse(document.getElementById('simulation-config').textContent);

const PAUSE_KEY = 'Space';
const DISPLAY_KEY = 'C';
const EMPTY_CELL = { displayChar: '.', displayEmoji: '🌿' };
const DEAD_ANIMAL = { displayChar: 'X', displayEmoji: '☠️' };
const HIGHLIGHTED_CELL_CLASS = 'highlighted';
const CONNECTED_TO_HUB_MESSAGE = 'Connected to game hub';
const DISPLAY_BUTTON_TEXT = { isEmojis: `Change to Characters (${DISPLAY_KEY})`, isCharacters: `Change to Emojis (${DISPLAY_KEY})`};
const PAUSE_BUTTON_TEXT = {paused: `Resume (${PAUSE_KEY})`, unpaused: `Pause (${PAUSE_KEY})`};

const connection = new signalR.HubConnectionBuilder()
    .withUrl(config.hubUrl)
    .build();

const pauseButton = document.getElementById(config.pauseId);
const displayButton = document.getElementById(config.toggleId);

let currentHighlightedCell = null;
let showEmojis = true;
let isPaused = true;
let lastData = null;

connection.on(config.gameUpdate, function (data) {
    lastData = data;
    updateBoard(data.field);
    updateHighlightedField(data.highlightRow, data.highlightColumn);
    updateDisplayedAnimal(data.highlightAnimal);
});

connection.start().then(function() {
    console.log(CONNECTED_TO_HUB_MESSAGE);
    if (config.simulationScreen) {
        pauseButton.textContent = isPaused ? PAUSE_BUTTON_TEXT.paused : PAUSE_BUTTON_TEXT.unpaused;
        displayButton.textContent = showEmojis ? DISPLAY_BUTTON_TEXT.isEmojis : DISPLAY_BUTTON_TEXT.isCharacters;
        connection.invoke(config.getStartState);
    }
});


function updateBoard(field)
{
    for (let row = 0; row < field.length; row++)
    {
        for (let column = 0; column < field[row].length; column++)
        {
            const cell = document.getElementById(formatCellId(row,column));
            if(cell)
            {
                const animal = field[row][column];
                let displayObject = !animal ? EMPTY_CELL : (animal.isAlive ? animal : DEAD_ANIMAL);
                cell.textContent = showEmojis ? displayObject.displayEmoji : displayObject.displayChar;
            }
        }
    }
}

function addAnimal(key) {
    connection.invoke(config.addAnimal,key);
}

document.addEventListener('keypress',function(e){
    if(document.activeElement.tagName === 'TEXTAREA' || document.activeElement.tagName === 'INPUT' || document.activeElement.isContentEditable){
        return;
    }
    console.log(e.key);
    if (e.key.toUpperCase() == DISPLAY_KEY) toggleDisplayMode();
    else if (e.code == PAUSE_KEY) togglePause();
    else if(document.getElementById(config.tableId)){
        addAnimal(e.key.toUpperCase());
    }
});

function handleCellClick(row, column)
{
    if (currentHighlightedCell == document.getElementById(formatCellId(row, column))) {
        removeHighlightedField()
        connection.invoke(config.handleCellUnclick);
    } else {
        updateHighlightedField(row,column);
        connection.invoke(config.handleCellClick, row, column);
    }
}

function updateHighlightedField(row,column){
    if (row !== null && column !== null) {
        if (currentHighlightedCell) {
            currentHighlightedCell.classList.remove(HIGHLIGHTED_CELL_CLASS);
        }
        let newCell = document.getElementById(formatCellId(row, column));
        if (newCell) {
            currentHighlightedCell = newCell;
            currentHighlightedCell.classList.add(HIGHLIGHTED_CELL_CLASS);
        }
    }
}

function removeHighlightedField() {
    currentHighlightedCell.classList.remove(HIGHLIGHTED_CELL_CLASS);
    currentHighlightedCell = null;
}

function updateDisplayedAnimal(animal) {
    const infoCard = document.getElementById(config.dataCardInfo.card);
    if (!animal) {
        if (infoCard.style.display == 'block') {
            infoCard.style.display = 'none';
            document.getElementById(config.dataCardInfo.icon).textContent = null;
            document.getElementById(config.dataCardInfo.name).textContent = null;
            document.getElementById(config.dataCardInfo.healthBar).style.width = null;
            document.getElementById(config.dataCardInfo.healthText).textContent = null;
            document.getElementById(config.dataCardInfo.staminaBar).style.width = null;
            document.getElementById(config.dataCardInfo.staminaText).textContent = null;
            document.getElementById(config.dataCardInfo.age).textContent = null;
            document.getElementById(config.dataCardInfo.offsprings).textContent = null;
        }
        return;
    } else {
        if (infoCard.style.display == 'none') {
            infoCard.style.display = 'block';
        }
        document.getElementById(config.dataCardInfo.icon).textContent = animal.displayEmoji;
        document.getElementById(config.dataCardInfo.name).textContent = animal.name;
        document.getElementById(config.dataCardInfo.healthBar).style.width = ((animal.health / animal.maxHealth) * 100) + "%";
        document.getElementById(config.dataCardInfo.healthText).textContent = `${animal.health.toFixed(2)} / ${animal.maxHealth.toFixed(2)}`;
        document.getElementById(config.dataCardInfo.staminaBar).style.width = ((animal.stamina / animal.maxStamina) * 100) + "%";
        document.getElementById(config.dataCardInfo.staminaText).textContent = `${animal.stamina.toFixed(2)} / ${animal.maxStamina.toFixed(2)}`;
        document.getElementById(config.dataCardInfo.age).textContent = animal.age.toFixed(2);
        document.getElementById(config.dataCardInfo.offsprings).textContent = animal.offsprings;
    }
    
}

function formatCellId(row,column){
    return config.cellIdFormat.replace('{0}', row).replace('{1}', column);
}
function formatButtonPress(button){
    return config.cellIdFormat.replace('{0}', button);
}

function toggleDisplayMode() {
    showEmojis = !showEmojis;
    displayButton.textContent = showEmojis ? DISPLAY_BUTTON_TEXT.isEmojis : DISPLAY_BUTTON_TEXT.isCharacters;
    updateBoard(lastData.field, lastData.highlightRow, lastData.highlightColumn);
}

function togglePause() {
    connection.invoke(config.togglePause);
    isPaused = !isPaused;
    pauseButton.textContent = isPaused ? PAUSE_BUTTON_TEXT.paused : PAUSE_BUTTON_TEXT.unpaused;
}
