import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/User';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.css']
})
export class MembersListComponent implements OnInit {

  users:User[];

  constructor(private UserService:UserService, private alertify:AlertifyService,
              private route:ActivatedRoute) { }

  ngOnInit() {
   this.route.data.subscribe(data=>{
     this.users = data['users']
   });
  }

  // loadUsers(){
  //   this.UserService.getUsers().subscribe((users:User[])=>{
  //     this.users = users;
  //   }, error =>{
  //      this.alertify.error('Failed to load users')
  //   })
  // }
}