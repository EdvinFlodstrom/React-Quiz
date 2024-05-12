import React, { useState } from 'react';
import Instructions from '../components/Instructions';
import InitializeQuiz from '../components/InitializeQuiz';
import CreateQuestion from '../components/CreateQuestion';
import ModifyQuestion from '../components/ModifyQuestion';
import DeleteQuestion from '../components/DeleteQuestion';
import '../styles/buttons.css';
import '../styles/initialPage.css';

const InitialPage = () => {
    const [componentToShow, setComponentToShow] = useState(null);
    const [, setAdjustGradient] = useState(true);

    function AdjustGradient() {
        setAdjustGradient((currentValue) => {
            const newValue = !currentValue;
            document.body.classList.toggle('moveGradient', currentValue);
            return newValue;
        });
    }

    return (
        <>
            {!componentToShow ? (
                <>
                    <Instructions />
                    <div className='options-button-container'>
                        <button
                            className='button take-quiz-button'
                            onClick={() => {
                                setComponentToShow(
                                    <InitializeQuiz
                                        adjustGradient={AdjustGradient}
                                    />
                                );
                                AdjustGradient();
                            }}>
                            Take Quiz
                        </button>

                        <button
                            className='button create-question-button'
                            onClick={() => {
                                setComponentToShow(
                                    <CreateQuestion
                                        adjustGradient={AdjustGradient}
                                    />
                                );
                                AdjustGradient();
                            }}>
                            Create Question
                        </button>

                        <button
                            className='button modify-question-button'
                            onClick={() => {
                                setComponentToShow(
                                    <ModifyQuestion
                                        adjustGradient={AdjustGradient}
                                    />
                                );
                                AdjustGradient();
                            }}>
                            Modify Question
                        </button>

                        <button
                            className='button delete-question-button'
                            onClick={() => {
                                setComponentToShow(
                                    <DeleteQuestion
                                        adjustGradient={AdjustGradient}
                                    />
                                );
                                AdjustGradient();
                            }}>
                            Delete Question
                        </button>
                    </div>
                </>
            ) : (
                componentToShow
            )}
        </>
    );
};

export default InitialPage;
