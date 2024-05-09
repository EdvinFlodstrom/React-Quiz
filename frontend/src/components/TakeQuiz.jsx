import React, { useRef, useState } from 'react';
import '../styles/timer.css';

const TakeQuiz = ({ playerName }) => {
    const [timerExpired, setTimerExpired] = useState(false);
    const [timerStarted, setTimerStarted] = useState(false);
    const [canGetQuestion, setCanGetQuestion] = useState(true);
    const [canSubmitAnswer, setCanSubmitAnswer] = useState(false);
    const [question, setQuestion] = useState('');
    const [questionOptions, setQuestionOptions] = useState({
        option1: '',
        option2: '',
        option3: '',
        option4: '',
    });
    let timerRef = useRef(null);

    const startTimer = () => {
        setTimerStarted(true);
        timerRef.current = setTimeout(() => {
            setTimerExpired(true);
            setTimerStarted(false);
            console.log('Time is up.');
        }, 15000);
    };

    const handleGetQuestion = () => {
        console.log(playerName);
    };

    const handleAnswer = (e) => {
        console.log('Stop timer.');
        setTimerExpired(true);
        setTimerStarted(false);
        clearTimeout(timerRef.current);
    };

    return (
        <>
            <h1>
                <span className='slide-in'>Take Quiz</span>
            </h1>

            <div className='timer'>
                <div className='timer-container'>
                    <div
                        className={`timer-bar ${
                            timerStarted ? 'timer--decrease' : 'timer-bar'
                        }`}></div>
                </div>
            </div>

            <div className='centered-buttons-container'>
                <button className='button'>Get Question</button>
                <h2>Q: {question}</h2>

                <div className='take-quiz-options-buttons-container'>
                    <button className='button take-quiz-options-button'>
                        Option 1: {questionOptions.option1}
                    </button>
                    <button className='button take-quiz-options-button'>
                        Option 2: {questionOptions.option2}
                    </button>
                    <button className='button take-quiz-options-button'>
                        Option 3: {questionOptions.option3}
                    </button>
                    <button className='button take-quiz-options-button'>
                        Option 4: {questionOptions.option4}
                    </button>
                </div>
            </div>
        </>
    );
};

export default TakeQuiz;
