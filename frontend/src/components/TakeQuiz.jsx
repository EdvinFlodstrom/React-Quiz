import React, { useRef, useState } from 'react';
import quizService from '../services/quizService';
import '../styles/timer.css';
import '../styles/takeQuiz.css';

const TakeQuiz = ({ playerName }) => {
    const [, setTimerExpired] = useState(false);
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
    const [correctOptionButton, setCorrectOptionButton] = useState({
        option1Button: '',
        option2Button: '',
        option3Button: '',
        option4Button: '',
    });
    const [timerDuration] = useState(10);
    let timerRef = useRef(null);

    const startTimer = () => {
        setTimerStarted(true);
        timerRef.current = setTimeout(() => {
            setTimerExpired(true);
            setTimerStarted(false);
            handleAnswer(0);
        }, timerDuration * 1000);
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
            const response = await quizService.getQuestion(playerName);

            const responseData = await response.data;

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
        } catch (exception) {
            const message =
                exception.response.data || exception.response.statusText;

            console.error('Error while getting question:', exception);
            setQuestion(message + ' Please try restarting the quiz.');
        }
    };

    const delay = (ms) => {
        return new Promise((resolve) => setTimeout(resolve, ms));
    };

    const handleAnswer = async (questionAnswer) => {
        setsubmitAnswerButtonDisabled(true);
        setTimerExpired(true);
        setTimerStarted(false);
        clearTimeout(timerRef.current);

        try {
            const response = await quizService.checkAnswer({
                playerName,
                questionAnswer,
            });

            const responseData = await response.data;

            if (responseData.correct) {
                setBackGroundFlash('flashGreen');
            } else {
                setBackGroundFlash('flashRed');
            }
            await delay(1000);

            setBackGroundFlash('');

            const correctOptionNumber = Number(responseData.correctOption);
            const correctOptionButtonKey = `option${correctOptionNumber}Button`;
            const chosenOptionButtonKey = `option${questionAnswer}Button`;
            setCorrectOptionButton((prevButtons) => ({
                ...prevButtons,
                [correctOptionButtonKey]: 'take-quiz-options-button-flashGreen',
                [chosenOptionButtonKey]:
                    questionAnswer === correctOptionNumber
                        ? 'take-quiz-options-button-flashGreen'
                        : 'take-quiz-options-button-flashRed',
            }));

            await delay(2500);
        } catch (exception) {
            const message =
                exception.response.data || exception.response.statusText;

            console.error('Error while checking answer:', message);
            setQuestion('Error while checking answer. Please try again.');
        } finally {
            setCorrectOptionButton({
                option1Button: '',
                option2Button: '',
                option3Button: '',
                option4Button: '',
            });
            setQuestionButtonDisabled(false);
        }
    };

    return (
        <div id='flash' className={backgroundFlash}>
            <div id='take-quiz-content'>
                <h1>
                    <span className='slide-in'>Take Quiz</span>
                </h1>

                <div className='timer'>
                    <div className='timer-container'>
                        <div
                            className={`timer-bar ${
                                timerStarted ? '' : 'timer-bar'
                            }`}
                            style={{
                                animation: timerStarted
                                    ? `timerAnimation ${timerDuration}s linear forwards`
                                    : 'none',
                            }}></div>
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
                            className={`button take-quiz-options-button ${correctOptionButton.option1Button}`}
                            onClick={() => handleAnswer(1)}
                            disabled={submitAnswerButtonDisabled}>
                            1: {questionOptions.option1}
                        </button>
                        <button
                            className={`button take-quiz-options-button ${correctOptionButton.option2Button}`}
                            onClick={() => handleAnswer(2)}
                            disabled={submitAnswerButtonDisabled}>
                            2: {questionOptions.option2}
                        </button>
                        <button
                            className={`button take-quiz-options-button ${correctOptionButton.option3Button}`}
                            onClick={() => handleAnswer(3)}
                            disabled={submitAnswerButtonDisabled}>
                            3: {questionOptions.option3}
                        </button>
                        <button
                            className={`button take-quiz-options-button ${correctOptionButton.option4Button}`}
                            onClick={() => handleAnswer(4)}
                            disabled={submitAnswerButtonDisabled}>
                            4: {questionOptions.option4}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default TakeQuiz;
