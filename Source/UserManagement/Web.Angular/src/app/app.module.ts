import { UserService } from './userManagement/user.service';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { UserComponent } from './userManagement/user.component';
import { VolunteerFormComponent } from './volunteerForm/volunteer-form.component';


@NgModule({
  declarations: [
    AppComponent,
    UserComponent,
    VolunteerFormComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    HttpModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [UserService],
  bootstrap: [AppComponent]
})
export class AppModule { }
