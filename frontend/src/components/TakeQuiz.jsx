import React, { useRef, useState } from 'react';
import '../styles/timer.css';
import '../styles/takeQuiz.css';

const TakeQuiz = ({ playerName }) => {
    const [timerExpired, setTimerExpired] = useState(false);
    const [timerStarted, setTimerStarted] = useState(false);
    const [getQuestionButtonDisabled, setQuestionButtonDisabled] =
        useState(false);
    const [submitAnswerButtonDisabled, setsubmitAnswerButtonDisabled] =
        useState(true);
    const [question, setQuestion] = useState('');
    const [questionOptions, setQuestionOptions] = useState({
        option1: '',
        option2: '',
        option3: '',
        option4: '',
    });
    const [backgroundFlash, setBackGroundFlash] = useState('');
    let timerRef = useRef(null);

    const startTimer = () => {
        setTimerStarted(true);
        timerRef.current = setTimeout(() => {
            setTimerExpired(true);
            setTimerStarted(false);
            handleAnswer(0);
        }, 10000);
    };

    const handleGetQuestion = async () => {
        setQuestionOptions({
            option1: '',
            option2: '',
            option3: '',
            option4: '',
        });
        setQuestionButtonDisabled(true);
        try {
            const response = await fetch(
                'https://localhost:7030/api/quiz/get',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        playerName: playerName,
                    }),
                }
            );

            if (!response.ok) {
                throw new Error('Failed to get question');
            }

            const responseData = await response.json();

            if (responseData.fourOptionQuestion) {
                // Question data is received
                setQuestion(responseData.fourOptionQuestion.question);

                await delay(3000);

                setQuestionOptions({
                    option1: responseData.fourOptionQuestion.option1,
                    option2: responseData.fourOptionQuestion.option2,
                    option3: responseData.fourOptionQuestion.option3,
                    option4: responseData.fourOptionQuestion.option4,
                });
                setsubmitAnswerButtonDisabled(false);
                startTimer(); // Start the timer shortly after receiving the question
            } else {
                // Question data is not received
                setQuestion(responseData.details);
            }
        } catch (error) {
            console.error('Error while getting question:', error);
        }
    };

    const delay = (ms) => {
        return new Promise((resolve) => setTimeout(resolve, ms));
    };

    const handleAnswer = async (answer) => {
        setsubmitAnswerButtonDisabled(true);
        setTimerExpired(true);
        setTimerStarted(false);
        clearTimeout(timerRef.current);
        try {
            const response = await fetch(
                'https://localhost:7030/api/quiz/answer',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        playerName: playerName,
                        questionAnswer: answer,
                    }),
                }
            );

            if (!response.ok) {
                throw new Error('Failed to check answer');
            }

            const responseData = await response.json();

            if (responseData.correct) {
                setBackGroundFlash('flashGreen');
            } else {
                setBackGroundFlash('flashRed');
            }
            await delay(1000);
            setBackGroundFlash('');
            setQuestionButtonDisabled(false);
        } catch (error) {
            console.error('Error while checking answer:', error);
        }
    };

    return (
        <div id='flash' className={backgroundFlash}>
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
                <button
                    className='button'
                    onClick={handleGetQuestion}
                    disabled={getQuestionButtonDisabled}>
                    Get Question
                </button>
                <h2>Q: {question}</h2>

                <div className='take-quiz-options-buttons-container'>
                    <button
                        className='button take-quiz-options-button'
                        onClick={() => handleAnswer(1)}
                        disabled={submitAnswerButtonDisabled}>
                        1: {questionOptions.option1}
                    </button>
                    <button
                        className='button take-quiz-options-button'
                        onClick={() => handleAnswer(2)}
                        disabled={submitAnswerButtonDisabled}>
                        2: {questionOptions.option2}
                    </button>
                    <button
                        className='button take-quiz-options-button'
                        onClick={() => handleAnswer(3)}
                        disabled={submitAnswerButtonDisabled}>
                        3: {questionOptions.option3}
                    </button>
                    <button
                        className='button take-quiz-options-button'
                        onClick={() => handleAnswer(4)}
                        disabled={submitAnswerButtonDisabled}>
                        4: {questionOptions.option4}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default TakeQuiz;
