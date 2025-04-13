from fastapi import FastAPI, HTTPException, Query
from f5_tts_mlx.generate import generate
from uuid import uuid4
import os

app = FastAPI()

@app.get("/")
async def generate_speech(text: str = Query(..., description="Text to convert to speech")):
    try:
        # Check if the reference audio file exists
        ref_audio_path = "./voice6_24.wav"
        if not os.path.exists(ref_audio_path):
            raise HTTPException(status_code=400, detail="Reference audio file not found.")

        # Call the f5_tts_mlx.generate function with the provided parameters
        print("Generating speech...")
        print(f"Text: {text}")
        uuid = uuid4()
        file_name = f"{uuid}.wav"
        output_path = f"./output/{file_name}"
        os.makedirs(os.path.dirname(output_path), exist_ok=True)
        print(f"Output path: {output_path}")
        generate(
            generation_text=text,
            ref_audio_path=ref_audio_path,
            ref_audio_text="Newton's first law of motion, also known as the law of inertia.",
            output_path=output_path,
            speed=1.0
        )

        # Return the file
        if os.path.exists(output_path):
            return {"message": "Speech generated successfully!", "status": "success", "file_name": file_name}
        else:
            raise HTTPException(status_code=500, detail="Failed to generate speech.")

    except Exception as e:
        return {"message": f"Error generating speech: {str(e)}", "status": "error"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
