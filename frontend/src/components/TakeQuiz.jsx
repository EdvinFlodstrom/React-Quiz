import React, { useRef, useState } from 'react';
import '../styles/timer.css';

const TakeQuiz = () => {
    const [timerExpired, setTimerExpired] = useState(false);
    const [timerStarted, setTimerStarted] = useState(false);
    let timerRef = useRef(null);

    const startTimer = () => {
        setTimerStarted(true);
        timerRef.current = setTimeout(() => {
            setTimerExpired(true);
            setTimerStarted(false);
            console.log('Time is up.');
        }, 15000);
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

            <button className='button' onClick={startTimer}>
                Start timer
            </button>

            <button className='button' onClick={handleAnswer}>
                Submit option 1 answer
            </button>
        </>
    );
};

export default TakeQuiz;
