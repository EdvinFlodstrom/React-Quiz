import React, { useState } from 'react';
import Instructions from '../components/Instructions';
import InitializeQuiz from '../components/InitializeQuiz';
import '../styles/buttons.css';

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
                    <button
                        className='button'
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
                </>
            ) : (
                componentToShow
            )}
        </>
    );
};

export default InitialPage;
